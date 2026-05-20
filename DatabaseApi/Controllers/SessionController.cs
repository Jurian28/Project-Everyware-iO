using DatabaseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Sessions;
using SharedClassLibrary.DTOs.Tags;
using System.Security.Claims;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("{eventId}/sessions")]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string noRoomErrorMessage = "Geen gekoppelde kamer";
        private readonly string noSpeakerErrorMessage = "Geen gekoppelde spreker";

        public SessionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<IEnumerable<SessionDTO>>>> GetAllSessions(int eventId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }


            List<SessionDTO> sessions = await _context.Sessions
                .Include(s => s.Room)
                .Include(s => s.Speakers)
                .Include(s => s.Tags)
                .Include(s => s.RegisteredUsers)
                .Where(s => s.IdEvent == eventId)
                .Select(s => new SessionDTO
                {
                    SessionId = s.IdSession,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Plenary = s.Plenary,
                    IsEnrolled = s.RegisteredUsers.Any(u => u.IdUser == userId),

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
                    SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
                    SpeakerName = s.Speakers
                        .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                        .FirstOrDefault() ?? noSpeakerErrorMessage
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<SessionDTO>>.Ok(sessions));
        }


        [HttpGet("getAdd")]
        public async Task<ActionResult<ApiResponse<CUSessionDTO>>> GetAddSessionData(int eventId)
        {
            CUSessionDTO availableSessionData = await GetFormOptions(eventId);

            return Ok(ApiResponse<CUSessionDTO>.Ok(availableSessionData));
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<ActionResult<ApiResponse<CUSessionDTO>>> GetEditSessionData(int eventId, int sessionId)
        {
            CUSessionDTO sessionData = await GetFormOptions(eventId);

            SessionDTO editingSession = await _context.Sessions
                .Include(s => s.Room)
                .Include(s => s.Speakers)
                .Include(s => s.Tags)
                .Where(s => s.IdSession == sessionId)
                .Select(s => new SessionDTO
                {
                    SessionId = s.IdSession,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Plenary = s.Plenary,
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
                    SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
                    SpeakerName = s.Speakers
                        .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                        .FirstOrDefault() ?? noSpeakerErrorMessage
                })
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

            var incomingTagIds = dto.Tags?.Select(t => t.IdTag).ToList() ?? new List<int>();

            session.Tags = await _context.Tags
                .Where(t => t.IdEvent == eventId && incomingTagIds.Contains(t.IdTag))
                .ToListAsync();

            session.Speakers ??= new List<Speaker>();
            session.Speakers.Clear();
            if (dto.SpeakerId.HasValue)
            {
                var speaker = await _context.Speakers.FindAsync(dto.SpeakerId.Value);
                if (speaker != null) session.Speakers.Add(speaker);
            }

            await _context.SaveChangesAsync();
            return StatusCode(201);
        }

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
    }
}