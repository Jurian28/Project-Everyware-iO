using DatabaseApi.DTOs;
using DatabaseApi.Models;
using DatabaseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Sessions;
using SharedClassLibrary.DTOs.Tags;
using System.Security.Claims;

namespace DatabaseApi.Controllers;

/// <summary>
/// Controller responsible for handling session enrollment operations, including retrieving session data,
/// enrolling users into sessions, and withdrawing users from sessions.
/// </summary>
[ApiController]
[Route("sessions")]
[Authorize]
public class SessionEnrollmentController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionEnrollmentController> _logger;
    private readonly SessionRegistrationService _sessionRegistrationService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionEnrollmentController"/> class.
    /// </summary>
    /// <param name="context">The database context used to access application data.</param>
    /// <param name="logger">The logger used for logging information and errors.</param1>
    public SessionEnrollmentController(ApplicationDbContext context, ILogger<SessionEnrollmentController> logger)
    {
        _context = context;
        _logger = logger;
        _sessionRegistrationService = new(context);
    }

    /// <summary>
    /// Retrieves the currently authenticated user's identifier from the claims principal.
    /// </summary>
    /// <returns>The authenticated user's identifier.</returns>
    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    /// <summary>
    /// Retrieves detailed information about a specific session, including room, speaker, tags,
    /// available places, and enrollment status for the authenticated user.
    /// </summary>
    /// <param name="sessionId">The identifier of the session to retrieve.</param>
    /// <returns>
    /// An HTTP response containing the session data if found, or a NotFound response if the session does not exist.
    /// </returns>
    [HttpGet("{sessionId}")]
    public async Task<ActionResult<ApiResponse<SessionDTO>>> GetSessionData(int sessionId)
    {
        string userId = GetUserId();

        Session? rawSession = await _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.Speakers)
            .Include(s => s.Tags)
            .Include(s => s.RegisteredUsers)
            .Where(s => s.IdSession == sessionId)
            .FirstOrDefaultAsync();

        if (rawSession == null)
        {
            Console.WriteLine($"Session with ID {sessionId} not found");
            return NotFound(ApiResponse<SessionDTO>.Fail("Session not found"));
        }

        User_has_Session? userRegistration = rawSession.RegisteredUsers.FirstOrDefault(u => u.IdUser == userId);
        bool inQueue = userRegistration?.InWaitingList ?? false;

        int? queuePosition = null;
        if (inQueue)
        {
            List<User_has_Session> waitingList = rawSession.RegisteredUsers
                .Where(u => u.InWaitingList)
                .OrderBy(u => u.JoinedDate)
                .ToList();
            int idx = waitingList.FindIndex(u => u.IdUser == userId);
            if (idx >= 0) queuePosition = idx + 1;
        }

        SessionDTO session = new SessionDTO
        {
            SessionId = rawSession.IdSession,
            Title = rawSession.Title,
            StartTime = rawSession.StartTime,
            EndTime = rawSession.EndTime,
            Plenary = rawSession.Plenary,
            PlacesLeft = rawSession.Room != null
                ? rawSession.Room.Capacity - rawSession.RegisteredUsers.Count(u => !u.InWaitingList)
                : 0,
            IsEnrolled = userRegistration != null,
            InQueue = inQueue,
            QueuePosition = queuePosition,
            Room = rawSession.Room != null
                ? new RoomResponseDTO
                {
                    IdRoom = rawSession.Room.IdRoom,
                    RoomLabel = rawSession.Room.RoomLabel,
                    Capacity = rawSession.Room.Capacity
                }
                : new RoomResponseDTO { RoomLabel = "No room" },
            Tags = rawSession.Tags.Select(t => new TagResponseDTO
            {
                IdTag = t.IdTag,
                IdEvent = t.IdEvent,
                Title = t.Title,
                ColorHex = t.ColorHex,
            }).ToList(),
            SpeakerId = rawSession.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
            SpeakerName = rawSession.Speakers
                .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                .FirstOrDefault() ?? "No speaker"
        };

        return Ok(ApiResponse<SessionDTO>.Ok(session));
    }

    /// <summary>
    /// Enrolls the currently authenticated user in the specified session.
    /// The endpoint validates:
    /// - that the session exists and is open for enrollment,
    /// - that the caller is authenticated and authorized,
    /// - that the user is not already enrolled,
    /// - and that enrolling does not create scheduling conflicts or exceed capacity.
    /// On success the endpoint returns a successful status with enrollment details.
    /// </summary>
    /// <param name="sessionId">Identifier of the session to enroll in.</param>
    /// <returns>
    /// 200 OK — enrollment succeeded; response body contains enrollment details.
    /// 400 Bad Request — invalid input (for example malformed sessionId).
    /// 400 Conflict — user is already enrolled in this or an overlapping session, or capacity/conflict prevents enrollment.
    /// 401 Unauthorized — caller is not authenticated.
    /// 403 Forbidden — caller is not permitted to enroll the specified user.
    /// 404 Not Found — session with the given id does not exist.
    /// 500 Internal Server Error — an unexpected error occurred.
    /// </returns>
    [HttpPost("{sessionId}/enroll")]
    public async Task<IActionResult> Enroll(int sessionId, bool overrideSessions = false)
    {
        User? user = _context.Users
                .Include(user => user.Events)
                .FirstOrDefault(user => user.UserName == User.Identity!.Name);
        if (user == null) return BadRequest(ApiResponse<object>.Fail("User not found"));

        Session? session = await _sessionRegistrationService.GetSession(sessionId);
        if (session == null) return NotFound(ApiResponse<object>.Fail("The session you tried to register for does not exist."));

        if (session.EndTime <= DateTime.UtcNow) return BadRequest(ApiResponse<object>.Fail("You can no longer register for this session because it has already ended."));

        User_has_Session? existingRegistration = await _sessionRegistrationService.GetExistingRegistration(user, session);
        if (existingRegistration != null) return BadRequest(ApiResponse<object>.Fail("You have already registered for this session."));

        List<User_has_Session> conflictingSessions = await _sessionRegistrationService.GetConflictingSessions(user, session);
        if (conflictingSessions.Count > 0 && !overrideSessions)
        {
            return BadRequest(ApiResponse<object>.Fail("You are already registered for an event on during that time.", new ConflictingSessionRegistrationDTO
            {
                Session = new ConflictingSessionDTO
                {
                    Id = session.IdSession,
                    Title = session.Title,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime
                },
                ConflictingSessions = conflictingSessions.Select(conflictingSession => new ConflictingSessionDTO
                {
                    Id = conflictingSession.IdSession,
                    Title = conflictingSession.Session.Title,
                    StartTime = conflictingSession.Session.StartTime,
                    EndTime = conflictingSession.Session.EndTime,
                    InQueue = conflictingSession.InWaitingList
                })
            }));
        }

        try
        {
            await _sessionRegistrationService.RegisterForSession(user, session, overrideSessions);
        }
        catch (Exception exception)
        {
            _logger.LogError("Failed to save new registration: {exception}", exception);
            return StatusCode(500, ApiResponse<object>.Fail("Something went wrong while trying to register for the session. Try again later."));
        }

        bool sessionFull = _sessionRegistrationService.IsSessionFull(session);
        return StatusCode(201, ApiResponse<object>.Ok(
            sessionFull
                ? "This session is full. Your registration is placed in the queue."
                : "You have registered yourself for this session."
        ));
    }

    /// <summary>
    /// Withdraws the authenticated user from a specific session enrollment.
    /// </summary>
    /// <remarks>
    /// Requires an authenticated user. This endpoint removes the current user's enrollment for the session
    /// identified by <paramref name="sessionId"/>. If the user is not enrolled in the specified session,
    /// the endpoint returns NotFound. On success the endpoint returns Ok.
    /// </remarks>
    /// <param name="sessionId">The identifier of the session to withdraw from.</param>
    /// <returns>
    /// 200 Ok when the withdrawal succeeds.
    /// 404 NotFound if the user is not enrolled or the session cannot be found.
    /// 401 Unauthorized if the request is unauthenticated.
    /// </returns>
    /// <response code="200">Enrollment removed successfully.</response>
    /// <response code="404">User not enrolled or session not found.</response>
    /// <response code="401">Authentication required.</response>
    [HttpDelete("{sessionId}/enroll")]
    public async Task<IActionResult> Withdraw(int sessionId)
    {
        Session? session = await _sessionRegistrationService.GetSession(sessionId);
        User? user = _context.Users
            .Include(user => user.Events)
            .FirstOrDefault(user => user.UserName == User.Identity!.Name);
        if (session == null || user == null) return BadRequest(ApiResponse<object>.Fail("User or session not found"));

        User_has_Session? registration = await _sessionRegistrationService.GetExistingRegistration(user, session);
        if (registration == null) return BadRequest(ApiResponse<object>.Fail("You do not have a registration for that session"));

        try
        {
            await _sessionRegistrationService.RemoveRegistration(registration, true);
            return Ok(ApiResponse<object>.Ok("Your registration has been successfully cancelled."));
        }
        catch (Exception exception)
        {
            _logger.LogError("Failed to remove registration: {exception}", exception);
            return StatusCode(500, ApiResponse<object>.Fail("Something went wrong while trying to cancel the registration. Try again later."));
        }
    }
}