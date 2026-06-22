using DatabaseApi.Models;
using DatabaseApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Sessions;
using SharedClassLibrary.DTOs.Tags;
using System.Security.Claims;
using DatabaseApi.Services;
using Microsoft.AspNetCore.Authorization;

namespace DatabaseApi.Controllers;

/// <summary>
/// Controller for managing sessions within an event, including retrieval, creation, updating, and deletion.
/// </summary>
[ApiController]
[Route("{eventId}/sessions")]
public class SessionController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SessionController> _logger;
    private readonly SessionRegistrationService _sessionRegistrationService;

    private readonly ISessionService _sessionService;
    private readonly static string noRoomErrorMessage = "Geen gekoppelde kamer";
    private readonly static string noSpeakerErrorMessage = "Geen gekoppelde spreker";

    public SessionController(ApplicationDbContext context, ILogger<SessionController> logger, ISessionService sessionService)
    {
        _context = context;
        _logger = logger;
        _sessionRegistrationService = new(_context);
        _sessionService = sessionService;
    }

    /// <summary>
    /// Retrieves all sessions for the specified event without user-specific enrollment data.
    /// </summary>
    /// <param name="eventId">The identifier of the event.</param>
    /// <returns>A list of sessions for the event.</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionDTO>>>> GetAllSessions(int eventId)
    {
        List<SessionDTO> sessions = await _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.Speakers)
            .Include(s => s.Tags)
            .Where(s => s.IdEvent == eventId)
            .Select(s => MapSessionToDto(s))
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<SessionDTO>>.Ok(sessions));
    }

    /// <summary>
    /// Retrieves all sessions for the specified event including enrollment and queue status for the authenticated user.
    /// </summary>
    /// <param name="eventId">The identifier of the event.</param>
    /// <returns>A list of sessions enriched with per-user enrollment data.</returns>
    [HttpGet("withUserData")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<IEnumerable<SessionDTO>>>> GetAllSessionsWithUserData(int eventId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        List<Session> rawSessions = await _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.Speakers)
            .Include(s => s.Tags)
            .Include(s => s.RegisteredUsers)
            .Where(s => s.IdEvent == eventId)
            .ToListAsync();

        TimeZoneInfo amsterdamZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam");

        List<SessionDTO> sessions = rawSessions.Select(s =>
        {
            User_has_Session? userReg = s.RegisteredUsers.FirstOrDefault(u => u.IdUser == userId);
            return new SessionDTO
            {
                SessionId = s.IdSession,
                Title = s.Title,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Plenary = s.Plenary,
                StartNotificationSendTime = s.StartNotificationSendTime == null
                    ? null
                    : TimeZoneInfo.ConvertTimeFromUtc((DateTime)s.StartNotificationSendTime, amsterdamZone),
                PlacesLeft = s.Room != null
                    ? s.Room.Capacity - s.RegisteredUsers.Count(u => !u.InWaitingList)
                    : 0,
                IsEnrolled = userReg != null,
                InQueue = userReg?.InWaitingList ?? false,
                Room = s.Room != null
                    ? new RoomResponseDTO
                    {
                        IdRoom = s.Room.IdRoom,
                        RoomLabel = s.Room.RoomLabel,
                        Capacity = s.Room.Capacity
                    }
                    : new RoomResponseDTO { RoomLabel = noRoomErrorMessage },
                Tags = s.Tags.Select(t => new TagResponseDTO
                {
                    IdTag = t.IdTag,
                    IdEvent = t.IdEvent,
                    Title = t.Title,
                    ColorHex = t.ColorHex,
                }).ToList(),
                SpeakerId = s.Speakers.Select(sp => sp.IdSpeaker).FirstOrDefault(),
                SpeakerName = s.Speakers
                    .Select(sp => $"{sp.FirstName} {sp.MiddleName} {sp.LastName}".Replace("  ", " ").Trim())
                    .FirstOrDefault() ?? noSpeakerErrorMessage
            };
        }).ToList();

        return Ok(ApiResponse<IEnumerable<SessionDTO>>.Ok(sessions));
    }

    /// <summary>
    /// Retrieves the data needed to populate the add-session form for the specified event.
    /// </summary>
    /// <param name="eventId">The identifier of the event.</param>
    /// <returns>Available rooms, tags, and speakers for the event.</returns>
    [HttpGet("getAdd")]
    public async Task<ActionResult<ApiResponse<CUSessionDTO>>> GetAddSessionData(int eventId)
    {
        CUSessionDTO availableSessionData = await GetFormOptions(eventId);

        return Ok(ApiResponse<CUSessionDTO>.Ok(availableSessionData));
    }

    /// <summary>
    /// Retrieves the data needed to populate the edit-session form, including the current session values.
    /// </summary>
    /// <param name="eventId">The identifier of the event.</param>
    /// <param name="sessionId">The identifier of the session to edit.</param>
    /// <returns>Available rooms, tags, and speakers, plus the current session data.</returns>
    [HttpGet("{sessionId}/edit")]
    public async Task<ActionResult<ApiResponse<CUSessionDTO>>> GetEditSessionData(int eventId, int sessionId)
    {
        CUSessionDTO sessionData = await GetFormOptions(eventId);

        SessionDTO editingSession = await _context.Sessions
            .Include(s => s.Room)
            .Include(s => s.Speakers)
            .Include(s => s.Tags)
            .Where(s => s.IdSession == sessionId)
            .Select(s => MapSessionToDto(s))
            .FirstOrDefaultAsync() ?? new SessionDTO();

        sessionData.session = editingSession;

        return Ok(ApiResponse<CUSessionDTO>.Ok(sessionData));
    }

    private async Task<CUSessionDTO> GetFormOptions(int eventId)
    {
        List<RoomResponseDTO> availableRooms = await _context.Rooms
            .Where(r => r.IdEvent == eventId)
            .Select(r => new RoomResponseDTO
            {
                IdRoom = r.IdRoom,
                RoomLabel = r.RoomLabel,
                Capacity = r.Capacity
            })
            .ToListAsync();

        List<TagResponseDTO> availableTags = await _context.Tags
            .Where(t => t.IdEvent == eventId)
            .Select(t => new TagResponseDTO
            {
                IdTag = t.IdTag,
                IdEvent = t.IdEvent,
                Title = t.Title,
                ColorHex = t.ColorHex
            })
            .ToListAsync();

        List<SessionSpeakerDTO> availableSpeakers = await _context.Speakers
            .Where(s => s.IdEvent == eventId)
            .Select(s => new SessionSpeakerDTO
            {
                SpeakerId = s.IdSpeaker,
                Name = $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim()
            })
            .ToListAsync();

        // is not allowed to be named event
        Event? events = await _context.Events
            .Where(e => e.IdEvent == eventId)
            .FirstOrDefaultAsync();

        CUSessionDTO sessionDTO = new CUSessionDTO
        {
            EventStartTime = events?.StartDate,
            EventEndTime = events?.EndDate,
            AvailableRooms = availableRooms,
            AvailableTags = availableTags,
            AvailableSpeakers = availableSpeakers
        };

        return sessionDTO;
    }

    /// <summary>
    /// Creates or updates a session. If the DTO contains a positive <c>SessionId</c> the existing session is updated; otherwise a new session is created.
    /// </summary>
    /// <param name="eventId">The identifier of the event the session belongs to.</param>
    /// <param name="dto">The session data to save.</param>
    /// <returns>201 on success, 404 if the session to update was not found.</returns>
    [HttpPost("save")]
    public async Task<IActionResult> SaveSession(int eventId, [FromBody] SessionDTO dto)
    {
        Session? session;
        if (dto.SessionId > 0)
        {
            session = await _context.Sessions
                .Include(s => s.Tags)
                .Include(s => s.Speakers)
                .FirstOrDefaultAsync(s => s.IdSession == dto.SessionId && s.IdEvent == eventId);

            if (session == null) return NotFound("Sessie niet gevonden.");
        }
        else
        {
            session = new Session { IdEvent = eventId, Speakers = new List<Speaker>() };
            _context.Sessions.Add(session);
        }

        session.Title = dto.Title;
        session.StartTime = dto.StartTime;
        session.EndTime = dto.EndTime;
        session.Plenary = dto.Plenary;
        session.IdRoom = dto.IdRoom;

        if (dto.StartNotificationSendTime != null)
        {
            DateTime utcTime = TimeZoneInfo.ConvertTimeToUtc((DateTime)dto.StartNotificationSendTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam"));
            session.StartNotificationSendTime = utcTime;
        }

        List<int> incomingTagIds = dto.Tags?.Select(t => t.IdTag).ToList() ?? new List<int>();

        session.Tags = await _context.Tags
            .Where(t => t.IdEvent == eventId && incomingTagIds.Contains(t.IdTag))
            .ToListAsync();

        session.Speakers ??= new List<Speaker>();
        session.Speakers.Clear();
        if (dto.SpeakerId.HasValue)
        {
            Speaker? speaker = await _context.Speakers.FindAsync(dto.SpeakerId.Value);
            if (speaker != null) session.Speakers.Add(speaker);
        }

        await _context.SaveChangesAsync();
        return StatusCode(201);
    }

    /// <summary>
    /// Deletes the specified session from the event.
    /// </summary>
    /// <param name="eventId">The identifier of the event.</param>
    /// <param name="sessionId">The identifier of the session to delete.</param>
    /// <returns>204 on success, 404 if not found, 500 on error.</returns>
    [HttpDelete("{sessionId}/delete")]
    public async Task<IActionResult> DeleteSession(int eventId, int sessionId)
    {
        Session? session = await _context.Sessions.FindAsync(sessionId);

        if (session == null) return NotFound();

        try
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
            return StatusCode(204);
        }
        catch
        {
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Get total,open and waitinglistspots for all sessions of event or 1 session if sessionId is given
    /// </summary>
    [HttpGet("GetSpotsData/{sessionId?}")]
    public async Task<ApiResponse<List<SessionSpotsDTO>>> GetSpotsData(int eventId, int? sessionId)
    {
        try
        {
            List<SessionSpotsDTO> DTOs = await _sessionService.ReturnSessionSpotsData(eventId, sessionId);
            return ApiResponse<List<SessionSpotsDTO>>.Ok(DTOs);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<SessionSpotsDTO>>.Fail(ex.ToString());
        }
    }

    private static SessionDTO MapSessionToDto(Session s)
    {
        return new()
        {
            SessionId = s.IdSession,
            Title = s.Title,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Plenary = s.Plenary,
            StartNotificationSendTime = 
            s.StartNotificationSendTime == null
                ? null
                : TimeZoneInfo.ConvertTimeFromUtc((DateTime)s.StartNotificationSendTime, TimeZoneInfo.FindSystemTimeZoneById("Europe/Amsterdam")),

            Room = s.Room != null
                ? new RoomResponseDTO
                {
                    IdRoom = s.Room.IdRoom,
                    RoomLabel = s.Room.RoomLabel,
                    Capacity = s.Room.Capacity
                }
                : new RoomResponseDTO { RoomLabel = noRoomErrorMessage },
            Tags = [.. s.Tags.Select(t => new TagResponseDTO
            {
                IdTag = t.IdTag,
                IdEvent = t.IdEvent,
                Title = t.Title,
                ColorHex = t.ColorHex,
            }) ],
            SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
            SpeakerName = s.Speakers
                                .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                                .FirstOrDefault() ?? noSpeakerErrorMessage
        };
    }
}

