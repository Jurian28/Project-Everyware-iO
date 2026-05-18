using DatabaseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Rooms;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="SessionEnrollmentController"/> class.
    /// </summary>
    /// <param name="context">The database context used to access application data.</param>
    public SessionEnrollmentController(ApplicationDbContext context)
    {
        _context = context;
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
                PlacesLeft = s.Room.Capacity - s.RegisteredUsers.Count(),
                IsEnrolled = s.RegisteredUsers
                    .Any(u => u.IdUser == userId),

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
    public async Task<IActionResult> Enroll(int sessionId)
    {
        string userId = GetUserId();
        User? user = await _context.Users.FindAsync(userId);

        if (user == null)
            return Unauthorized();

        Session? session = await _context.Sessions
            .Include(s => s.RegisteredUsers)
            .FirstOrDefaultAsync(s => s.IdSession == sessionId);

        if (session == null)
            return NotFound("Session not found");

        Session? conflictingSession = await _context.Sessions
            .Where(s => s.RegisteredUsers.Any(u => u.User.Id == userId))
            .Where(s =>
                s.StartTime < session.EndTime &&
                session.StartTime < s.EndTime)
            .FirstOrDefaultAsync();

        if (conflictingSession != null)
        {
            return Conflict(new
            {
                message = "You already have a session at this time",
                conflictSession = new
                {
                    conflictingSession.IdSession,
                    conflictingSession.Title,
                    conflictingSession.StartTime,
                    conflictingSession.EndTime
                }
            });
        }

        session.RegisteredUsers.Add(new User_has_Session
        {
            IdUser = userId,
            IdSession = session.IdSession,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return Ok();
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
        string userId = GetUserId();

        User_has_Session? userInSession = await _context.User_has_Sessions
            .FirstOrDefaultAsync(x =>
                x.IdSession == sessionId &&
                x.IdUser == userId);

        if (userInSession == null)
            return NotFound();

        _context.User_has_Sessions.Remove(userInSession);
        await _context.SaveChangesAsync();

        return Ok();
    }
}