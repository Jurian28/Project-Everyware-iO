using System.Text.Json;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<IEnumerable<SessionDTO>>> GetAllSessions(int eventId)
        {
            List<SessionDTO> sessions = await _context.Sessions
                .Include(s => s.Room)
                .Include(s => s.Speakers)
                .Include(s => s.Tags)
                .Where(s => s.IdEvent == eventId)
                .Select(s => new SessionDTO
                {
                    SessionId = s.IdSession,
                    Title = s.Title,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Plenary = s.Plenary,
                    Capacity = s.Capacity,
                    IdRoom = s.IdRoom,
                    RoomName = s.Room.RoomLabel ?? noRoomErrorMessage,
                    Tags = s.Tags.Select(t => new SessionTagDTO
                    {
                        EventId = t.IdEvent,
                        Title = t.Title
                    }).ToList(),
                    SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
                    SpeakerName = s.Speakers
                        .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                        .FirstOrDefault() ?? noSpeakerErrorMessage
                })
                .ToListAsync();

            return Ok(sessions);
        }

        [HttpGet("add")]
        public async Task<ActionResult<CUSessionDTO>> GetAvailableRoomsTagsSpeakers(int eventId)
        {
            CUSessionDTO availableSessionData = await AvailableRoomsTagsSpeakers(eventId);

            return Ok(availableSessionData);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<ActionResult<CUSessionDTO>> GetEditingSession(int eventId, int sessionId)
        {
            CUSessionDTO sessionData = await AvailableRoomsTagsSpeakers(eventId);

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
                    Capacity = s.Capacity,
                    IdRoom = s.IdRoom,
                    RoomName = s.Room.RoomLabel ?? noRoomErrorMessage,
                    Tags = s.Tags.Select(t => new SessionTagDTO
                    {
                        EventId = t.IdEvent,
                        Title = t.Title
                    }).ToList(),
                    SpeakerId = s.Speakers.Select(s => s.IdSpeaker).FirstOrDefault(),
                    SpeakerName = s.Speakers
                        .Select(s => $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim())
                        .FirstOrDefault() ?? noSpeakerErrorMessage
                })
                .FirstOrDefaultAsync() ?? new SessionDTO();

            sessionData.session = editingSession;

            return Ok(sessionData);
        }

        private async Task<CUSessionDTO> AvailableRoomsTagsSpeakers(int eventId)
        {
            List<SessionRoomDTO> availableRooms = await _context.Rooms
                .Where(r => r.IdEvent == eventId)
                .Select(r => new SessionRoomDTO
                {
                    RoomId = r.IdRoom,
                    RoomLabel = r.RoomLabel,
                    Capacity = r.Capacity
                })
                .ToListAsync();

            List<SessionTagDTO> availableTags = await _context.Tags
                .Where(t => t.IdEvent == eventId)
                .Select(t => new SessionTagDTO
                {
                    EventId = t.IdEvent,
                    Title = t.Title
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

            CUSessionDTO sessionDTO = new CUSessionDTO
            {
                AvailableRooms = availableRooms,
                AvailableTags = availableTags,
                AvailableSpeakers = availableSpeakers
            };

            return sessionDTO;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveSession(int eventId, [FromBody] SessionDTO dto)
        {
            Session session;
            Console.WriteLine("database:");
            Console.WriteLine(JsonSerializer.Serialize(dto));

            if (dto.SessionId > 0)
            {
                session = await _context.Sessions
                    .Include(s => s.Tags)
                    .Include(s => s.Speakers)
                    .FirstOrDefaultAsync(s => s.IdSession == dto.SessionId && s.IdEvent == eventId);

                // Verwijder de ?? new Session regel hierboven, doe de check apart:
                if (session == null) return NotFound("Sessie niet gevonden.");
            }
            else
            {
                session = new Session { IdEvent = eventId, Speakers = new List<Speaker>() };
                _context.Sessions.Add(session);
            }

            // Basis velden mappen
            session.Title = dto.Title;
            session.StartTime = dto.StartTime;
            session.EndTime = dto.EndTime;
            session.Plenary = dto.Plenary;
            session.Capacity = dto.Plenary ? null : dto.Capacity;
            session.IdRoom = dto.IdRoom ?? 0;

            // --- Relaties bijwerken ---

            // 1. Pak de titels uit de DTO
            var incomingTagTitles = dto.Tags?.Select(t => t.Title).ToList() ?? new List<string>();

            // 2. Zoek de tags op die EN de juiste titel hebben EN bij dit event horen
            session.Tags = await _context.Tags
                .Where(t => t.IdEvent == eventId && incomingTagTitles.Contains(t.Title))
                .ToListAsync();

            // Speakers
            session.Speakers ??= new List<Speaker>();
            session.Speakers.Clear();
            if (dto.SpeakerId.HasValue)
            {
                var speaker = await _context.Speakers.FindAsync(dto.SpeakerId.Value);
                if (speaker != null) session.Speakers.Add(speaker);
            }

            await _context.SaveChangesAsync();
            return Ok(new { sessionId = session.IdSession });
        }
    }
}