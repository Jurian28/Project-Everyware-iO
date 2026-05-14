using DatabaseApi.Models;
using DatabaseApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Sessions;

namespace DatabaseApi.Services;

public class SessionService : ISessionService
{
    private readonly ApplicationDbContext _context;

    public SessionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<List<SessionSpotsDTO>> ReturnSessionSpotsData(int eventId, int? sessionId)
    {
        var query = _context.Sessions
        .Include(s => s.Room)
        .Where(s => s.IdEvent == eventId)
        .AsQueryable();

        if (sessionId != null)
        {
            query = query.Where(s => s.IdSession == sessionId);
        }

        return await query.Select(s => new SessionSpotsDTO
        {
            SessionId = s.IdSession,
            TotalSpots = s.Room.Capacity,
            FilledSpots = s.RegisteredUsers.Count(o => o.InWaitingList == false),
            SpotsInWaitingList = s.RegisteredUsers.Count(o => o.InWaitingList == true),
            IsPlenarySession = s.Plenary
        }).ToListAsync();
    }
}
