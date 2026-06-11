using DatabaseApi.Models;
using DatabaseApi.Services;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.BackgroundServices;

public class StartNotificationsService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    private static readonly string SESSION_NOTIFICATION_TITLE = "Session '{0}' is starting soon";
    private static readonly string SESSION_NOTIFICATION_CONTENT = "The session is starting at {0}.";
    private static readonly uint TIMER_INTERVAL_SECONDS = 30;

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
        using IServiceScope scope = _scopeFactory.CreateScope();
        ApplicationDbContext context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        NotificationService notificationService = new(context);

        DateTime now = DateTime.Now;
        List<Session> sessions = await context.Sessions
            .Include(session => session.RegisteredUsers)
                .ThenInclude(userHasSession => userHasSession.User)
            .Where(session => !session.StartingNotificationSent && session.StartNotificationSendTime <= now)
            .ToListAsync(cancellationToken);

        foreach (Session session in sessions)
        {
            string title = string.Format(SESSION_NOTIFICATION_TITLE, session.Title);
            string content = string.Format(SESSION_NOTIFICATION_CONTENT, session.StartTime);

            foreach (User user in session.RegisteredUsers.Select(userHasSession => userHasSession.User))
                await notificationService.SendNotification(user, title, content);

            session.StartingNotificationSent = true;
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

