using DatabaseApi.Models;

namespace DatabaseApi.Services;

public class NotificationService(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Notification>> GetNotificationsForUser(string userId)
    {
        return _context.Notifications
            .Where(notification => notification.Receiver.Id == userId)
            .ToList();
    }

    public async Task SendNotification(User receiver, string title, string content)
    {
        _context.Notifications.Add(new()
        {
            Receiver = receiver,
            Title = title,
            Content = content,
            SentAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }
}

