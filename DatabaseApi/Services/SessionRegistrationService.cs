using DatabaseApi.Exceptions;
using DatabaseApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Services;

public class SessionRegistrationService
{
    private readonly ApplicationDbContext _context;
    private readonly NotificationService _notificationService;

    public SessionRegistrationService(ApplicationDbContext context)
    {
        _context = context;
        _notificationService = new(context);
    }

    public async Task<Session?> GetSession(int sessionId)
    {
        return await _context.Sessions
            .Include(session => session.RegisteredUsers)
            .Include(session => session.Room)
            .FirstOrDefaultAsync(session => session.IdSession == sessionId);
    }

    public async Task<List<User_has_Session>> GetConflictingSessions(User user, Session session)
    {
        return await _context.User_has_Sessions
            .Include(userHasSession => userHasSession.Session)
            .Where(userHasSession => userHasSession.IdUser == user.Id)
            .Where(userHasSession => userHasSession.IdSession != session.IdSession)
            .Where(userHasSession => userHasSession.Session.StartTime < session.EndTime && session.StartTime < userHasSession.Session.EndTime)
            .ToListAsync();
    }

    public async Task<User_has_Session?> GetExistingRegistration(User user, Session session)
    {
        return await _context.User_has_Sessions
            .Where(userHasSession => userHasSession.IdSession == session.IdSession)
            .Where(userHasSession => userHasSession.IdUser == user.Id)
            .FirstOrDefaultAsync();
    }

    public bool IsSessionFull(Session session)
    {
        int currentRegistrationCount = session.RegisteredUsers.Count;
        int roomCapacity = session.Room.Capacity;

        return currentRegistrationCount >= roomCapacity;
    }

    public async Task<List<User_has_Session>> GetQueuedRegistrations(Session session)
    {
        return [.. _context.User_has_Sessions
            .Include(userHasSession => userHasSession.User)
            .Where(registeredUser => registeredUser.InWaitingList)
            .OrderByDescending(registeredUser => registeredUser.JoinedDate) ];
    }

    public async Task RegisterForSession(User user, Session session, bool overrideConflictingSessions)
    {
        List<User_has_Session> conflictingSessions = await GetConflictingSessions(user, session);

        if (conflictingSessions.Count > 0)
        {
            if (!overrideConflictingSessions)
            {
                throw new RegistrationConflictingSessionsException();
            }

            _context.User_has_Sessions.RemoveRange(conflictingSessions);
        }

        bool placeInQueue = IsSessionFull(session);

        _context.User_has_Sessions.Add(new User_has_Session
        {
            IdSession = session.IdSession,
            IdUser = user.Id,
            InWaitingList = placeInQueue,
            JoinedDate = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task RemoveRegistration(User_has_Session registration, bool registerFirstFromQueue)
    {
        _context.User_has_Sessions.Remove(registration);

        if (registerFirstFromQueue)
        {
            await RegisterFirstFromQueue(registration.Session);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RegisterFirstFromQueue(Session session)
    {
        List<User_has_Session> queuedRegistrations = await GetQueuedRegistrations(session);

        if (queuedRegistrations.Count > 0 && session.EndTime > DateTime.UtcNow)
        {
            User_has_Session oldestRegistration = queuedRegistrations.First();
            oldestRegistration.InWaitingList = false;
            await _context.SaveChangesAsync();

            string title = $"Enrolled for session {oldestRegistration.Session.Title}";
            string content = $"A spot has opened for session '{oldestRegistration.Session.Title}'. You have been enrolled for this session.";
            await _notificationService.SendNotification(oldestRegistration.User, title, content);
        }
    }
}

