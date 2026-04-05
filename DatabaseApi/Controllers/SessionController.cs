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
            var sessions = await _context.Sessions
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
            var sessionData = await AvailableRoomsTagsSpeakers(eventId);

            return Ok(sessionData);
        }

        [HttpGet("{sessionId}/edit")]
        public async Task<ActionResult<CUSessionDTO>> GetEditingSession(int eventId, int sessionId)
        {
            var sessionData = await AvailableRoomsTagsSpeakers(eventId);

            return Ok(sessionData);
        }

        private async Task<ActionResult<CUSessionDTO>> AvailableRoomsTagsSpeakers(int eventId)
        {
            var availableRooms = await _context.Rooms
                .Where(r => r.IdEvent == eventId)
                .Select(r => new SessionRoomDTO
                {
                    RoomId = r.IdRoom,
                    RoomLabel = r.RoomLabel,
                    Capacity = r.Capacity
                })
                .ToListAsync();

            var availableTags = await _context.Tags
                .Where(t => t.IdEvent == eventId)
                .Select(t => new SessionTagDTO
                {
                    EventId = t.IdEvent,
                    Title = t.Title
                })
                .ToListAsync();

            var availableSpeakers = await _context.Speakers
                .Where(s => s.IdEvent == eventId)
                .Select(s => new SessionSpeakerDTO
                {
                    SpeakerId = s.IdSpeaker,
                    Name = $"{s.FirstName} {s.MiddleName} {s.LastName}".Replace("  ", " ").Trim()
                })
                .ToListAsync();

            CUSessionDTO session = new CUSessionDTO
            {
                AvailableRooms = availableRooms,
                AvailableTags = availableTags,
                AvailableSpeakers = availableSpeakers
            };

            return Ok(session);
        }
    }
}