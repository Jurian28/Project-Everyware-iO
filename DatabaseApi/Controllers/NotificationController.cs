using DatabaseApi.Models;
using DatabaseApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs;

namespace DatabaseApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class NotificationController(ApplicationDbContext context) : Controller
{
    private readonly NotificationService _notificationService = new(context);

    [HttpGet("get-notifications")]
    public async Task<ActionResult<ApiResponse<IEnumerable<NotificationDTO>>>> GetNotifications()
    {
        User? user = context.Users
            .Include(user => user.Events)
            .FirstOrDefault(user => user.UserName == User.Identity!.Name);

        if (user == null)
        {
            return NotFound(ApiResponse<IEnumerable<NotificationDTO>>.Fail("User not found"));
        }

        IEnumerable<Notification> notifications = await _notificationService.GetNotificationsForUser(user.Id);
        IEnumerable<NotificationDTO> notificationDtos = notifications.Select(notification => new NotificationDTO()
        {
            Id = notification.Id,
            Title = notification.Title,
            Content = notification.Content
        });

        return Ok(ApiResponse<IEnumerable<NotificationDTO>>.Ok(notificationDtos));
    }

    [HttpPost("mark-as-read/{notificationId}")]
    public async Task<ActionResult> MarkAsRead(int notificationId)
    {
        User? user = context.Users
            .Include(user => user.Events)
            .FirstOrDefault(user => user.UserName == User.Identity!.Name);

        Notification? notification = context.Notifications
            .Include(notification => notification.Receiver)
            .FirstOrDefault(notification => notification.Id == notificationId);

        if (notification == null || user == null || notification.Receiver.Id != user.Id)
        {
            return NotFound();
        }

        await _notificationService.DeleteNotification(notification);
        return Ok();
    }
}

