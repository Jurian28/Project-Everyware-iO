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

        SessionDTO? session = await _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.Speakers)
            .Include(s => s.Tags)
            .Include(s => s.RegisteredUsers)
            .Where(s => s.IdSession == sessionId)
            .Select(s => new SessionDTO
            {
                SessionId = s.IdSession,
                Title = s.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Plenary = s.Plenary,
                PlacesLeft = s.Room.Capacity - s.RegisteredUsers.Count(registeredUser => !registeredUser.InWaitingList),
                IsEnrolled = s.RegisteredUsers
                    .Any(u => u.IdUser == userId),
                InQueue = s.RegisteredUsers
                    .Where(u => u.IdUser == userId)
                    .Select(u => u.InWaitingList)
                    .FirstOrDefault(),

                Room = s.Room != null
                    ? new RoomResponseDTO
                    {
                        IdRoom = s.Room.IdRoom,
                        RoomLabel = s.Room.RoomLabel,
                        Capacity = s.Room.Capacity
                    }
                    : new RoomResponseDTO { RoomLabel = "No room" },
                Tags = s.Tags.Select(t => new TagResponseDTO
                {
                    IdTag = t.IdTag,
                    IdEvent = t.IdEvent,
                    Title = t.Title,
                    ColorHex = t.ColorHex,
                }).ToList(),
                SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
                SpeakerName = s.Speakers
                    .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}")
                    .FirstOrDefault() ?? "No speaker"
            })
            .FirstOrDefaultAsync();

        if (session == null)
        {
            Console.WriteLine($"Session with ID {sessionId} not found");
            return NotFound(ApiResponse<SessionDTO>.Fail("Session not found"));
        }

        return Ok(ApiResponse<SessionDTO>.Ok(session));
    }

    /// <summary>
    /// Enrolls the authenticated user into a specific session if no scheduling conflicts exist.
    /// </summary>
    /// <param name="sessionId">The identifier of the session to enroll in.</param>
    /// <returns>
    /// An HTTP response indicating the result of the enrollment operation.
    /// Returns Conflict if the user is already enrolled in an overlapping session.
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
    /// <param name="sessionId">The identifier of the session to withdraw from.</param>
    /// <returns>
    /// An HTTP response indicating the result of the withdrawal operation.
    /// Returns NotFound if the user is not enrolled in the session.
    /// </returns>
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