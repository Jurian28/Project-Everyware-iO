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
                    TagNames = s.Tags.Select(t => t.Title).ToList(),
                    SpeakerName = s.Speakers
                        .Select(t => $"{t.FirstName} {t.MiddleName} {t.LastName}".Replace("  ", " ").Trim())
                        .FirstOrDefault() ?? noSpeakerErrorMessage
                })
                .ToListAsync();

            return Ok(sessions);
        }
    }
}