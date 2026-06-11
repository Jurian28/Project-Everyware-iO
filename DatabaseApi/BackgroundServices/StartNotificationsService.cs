using DatabaseApi.Models;
using DatabaseApi.Services;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.BackgroundServices;

public class StartNotificationsService : BackgroundService
{
    private readonly NotificationService _notificationService;
    private readonly ApplicationDbContext _context;

    private static readonly string SESSION_NOTIFICATION_TITLE = "Session '{}' is starting soon";
    private static readonly string SESSION_NOTIFICATION_CONTENT = "The session is starting at {}.";
    private static readonly uint TIMER_INTERVAL_SECONDS = 30;

    public StartNotificationsService(IServiceScopeFactory scopeFactory)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _notificationService = new(_context);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await CheckSessions(cancellationToken);
            await Task.Delay(TimeSpan.FromSeconds(TIMER_INTERVAL_SECONDS), cancellationToken);
        }
    }

    private async Task CheckSessions(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.Now;
        List<Session> sessions = await _context.Sessions
            .Include(session => session.RegisteredUsers)
                .ThenInclude(userHasSession => userHasSession.User)
            .Where(session => !session.StartingNotificationSent && session.StartTime - session.NotificationLeadTime <= now)
            .ToListAsync(cancellationToken);

        foreach (Session session in sessions)
        {
            string title = string.Format(SESSION_NOTIFICATION_TITLE, session.Title);
            string content = string.Format(SESSION_NOTIFICATION_CONTENT, session.StartTime);

            foreach (User user in session.RegisteredUsers.Select(x => x.User))
                await _notificationService.SendNotification(user, title, content);

            session.StartingNotificationSent = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}

