using System.Text.Json;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Tags;
using SharedClassLibrary.DTOs.Sessions;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("{eventId}/sessions")]
    public class SessionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SessionController> _logger;
        
        private readonly string noRoomErrorMessage = "Geen gekoppelde kamer";
        private readonly string noSpeakerErrorMessage = "Geen gekoppelde spreker";

        public SessionController(ApplicationDbContext context, ILogger<SessionController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SessionDTO>>>> GetAllSessions(int eventId)
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

        [HttpPost("{sessionId}/register")]
        [Authorize]
        public async Task<IActionResult> RegisterForSession(int eventId, int sessionId, bool overrideSessions = false)
        {
            User? user = _context.Users
                .Include(user => user.Events)
                .FirstOrDefault(user => user.UserName == User.Identity!.Name);

            if (user == null) return BadRequest("User not found");

            User_has_Session? existingRegistration = await _context.User_has_Sessions
                .Where(userHasSession => userHasSession.IdSession == sessionId)
                .Where(userHasSession => userHasSession.IdUser == user.Id)
                .FirstOrDefaultAsync();

            if (existingRegistration != null) return BadRequest("You have already registered for this session.");

            Session? session = await _context.Sessions
                .Include(session => session.RegisteredUsers)
                .Include(session => session.Room)
                .FirstOrDefaultAsync(session => session.IdSession == sessionId);

            if (session == null) return NotFound("The session you tried to register for does not exist.");

            List<User_has_Session> conflictingSessions = await _context.User_has_Sessions
                .Include(userHasSession => userHasSession.Session)
                .Where(userHasSession => userHasSession.IdUser == user.Id)
                .Where(userHasSession => userHasSession.IdSession != session.IdSession)
                .Where(userHasSession => userHasSession.Session.StartTime < session.EndTime && session.StartTime < userHasSession.Session.EndTime)
                .ToListAsync();

            if (conflictingSessions.Count > 0)
            {
                if (overrideSessions)
                {
                    _context.User_has_Sessions.RemoveRange(conflictingSessions);
                }
                else
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Data = new
                        {
                            Session = new
                            {
                                Id = session.IdSession,
                                session.Title,
                                session.StartTime,
                                session.EndTime,
                            },
                            ConflictingSessions = conflictingSessions.Select(conflictingSession => new
                            {
                                Id = conflictingSession.IdSession,
                                conflictingSession.Session.Title,
                                conflictingSession.Session.StartTime,
                                conflictingSession.Session.EndTime
                            })
                        },
                        Error = "You are already registered for an event on during that time."
                    });
                }
            }

            int currentRegistrationCount = session.RegisteredUsers.Count;
            int roomCapacity = session.Room.Capacity;
            bool registerInQueue = currentRegistrationCount >= roomCapacity;

            _context.User_has_Sessions.Add(new User_has_Session
            {
                IdSession = session.IdSession,
                IdUser = user.Id,
                InWaitingList = registerInQueue,
                JoinedDate = DateTime.UtcNow
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to save new registration: {exception}", exception);
                return StatusCode(500);
            }

            return StatusCode(201, new
            {
                Success = true,
                Data = (object?)null,
                Error = (object?)null,
                Message = registerInQueue
                    ? "This session is full. Your registration is placed in the queue."
                    : "You have registered yourself for this session."
            });
        }

        [HttpPost("{sessionId}/cancel-registration")]
        [Authorize]
        public async Task<IActionResult> CancelRegistration(int eventId, int sessionId)
        {
            Session? session = await _context.Sessions
                .Include(session => session.RegisteredUsers)
                .Include(session => session.Room)
                .FirstOrDefaultAsync(session => session.IdSession == sessionId);
            User? user = _context.Users
                .Include(user => user.Events)
                .FirstOrDefault(user => user.UserName == User.Identity!.Name);

            if (session == null || user == null) return BadRequest("User or session not found");

            User_has_Session? registration = session.RegisteredUsers
                .FirstOrDefault(registration => registration.IdUser == user.Id && registration.IdSession == session.IdSession);

            if (registration == null) return BadRequest("You do not have a registration for that session");

            _context.User_has_Sessions.Remove(registration);

            List<User_has_Session> queuedRegistrations = session.RegisteredUsers
                .Where(registeredUser => registeredUser.InWaitingList)
                .Where(registeredUser => registeredUser.IdUser != user.Id)
                .OrderByDescending(registeredUser => registeredUser.JoinedDate)
                .ToList();

            if (queuedRegistrations.Count > 0 && session.EndTime > DateTime.UtcNow)
            {
                User_has_Session oldestRegistration = queuedRegistrations.First();
                oldestRegistration.InWaitingList = false;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                _logger.LogError("Failed to remove registration: {exception}", exception);
                return StatusCode(500);
            }

            return Ok();
        }
    }
}