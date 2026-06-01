using Microsoft.AspNetCore.Mvc;
using DatabaseApi.DTOs;
using DatabaseApi.Models;
using DatabaseApi.Services;

namespace DatabaseApi.Controllers;

[ApiController]
[Route("[controller]")]
public class NotificationController(ApplicationDbContext context) : Controller
{
    private readonly NotificationService _notificationService = new(context);

    [HttpGet("get-notifications-for-user/{userId}")]
    public async Task<ApiResponse<IEnumerable<NotificationDTO>>> GetNotificationsForUser(string userId)
    {
        IEnumerable<Notification> notifications = await _notificationService.GetNotificationsForUser(userId);
        IEnumerable<NotificationDTO> notificationDtos = notifications.Select(notification => new NotificationDTO()
        {
            Title = notification.Title,
            Content = notification.Content
        });

        return ApiResponse<IEnumerable<NotificationDTO>>.Ok(notificationDtos);
    }
}

