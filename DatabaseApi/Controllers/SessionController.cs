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
    }
}