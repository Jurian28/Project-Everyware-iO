using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs;

namespace UnitTests.Controllers;

[TestClass]
public class NotificationControllerTests : ControllerTestBase {
    private ApplicationDbContext CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        User user = TestHelpers.CreateTestUser("user1", "testuser", "test@test.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        User savedUser = ctx.Users.First(u => u.Id == "user1");
        ctx.Notifications.AddRange(
            new Notification { Receiver = savedUser, Title = "Title1", Content = "Content1", SentAt = DateTime.UtcNow },
            new Notification { Receiver = savedUser, Title = "Title2", Content = "Content2", SentAt = DateTime.UtcNow }
        );
        ctx.SaveChanges();
        return ctx;
    }

    [TestMethod]
    public async Task GetNotifications_ReturnsUserNotifications()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        NotificationController controller = new NotificationController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ActionResult<ApiResponse<IEnumerable<NotificationDTO>>> result = await controller.GetNotifications();

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<NotificationDTO>>? response = okResult.Value as ApiResponse<IEnumerable<NotificationDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(2, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetNotifications_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("NotifUserNotFound");
        NotificationController controller = new NotificationController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("nonexistent", "nouser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ActionResult<ApiResponse<IEnumerable<NotificationDTO>>> result = await controller.GetNotifications();

        // Assert
        NotFoundObjectResult? notFound = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task MarkAsRead_ValidNotification_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("NotifMarkRead");
        User user = TestHelpers.CreateTestUser("user2", "reader", "r@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.First(u => u.Id == "user2");
        Notification notif = new Notification { Receiver = savedUser, Title = "T", Content = "C", SentAt = DateTime.UtcNow };
        ctx.Notifications.Add(notif);
        ctx.SaveChanges();

        NotificationController controller = new NotificationController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user2", "reader");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult result = await controller.MarkAsRead(notif.Id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(OkResult));
        Assert.IsNull(await ctx.Notifications.FindAsync(notif.Id));
    }

    [TestMethod]
    public async Task MarkAsRead_WrongUser_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("NotifWrongUser");
        User owner = TestHelpers.CreateTestUser("owner", "owner", "o@t.com");
        ctx.Users.Add(owner);
        ctx.SaveChanges();
        User savedOwner = ctx.Users.First(u => u.Id == "owner");
        Notification notif = new Notification { Receiver = savedOwner, Title = "T", Content = "C", SentAt = DateTime.UtcNow };
        ctx.Notifications.Add(notif);
        ctx.SaveChanges();

        NotificationController controller = new NotificationController(ctx);
        ClaimsPrincipal otherUser = TestHelpers.CreateClaimsPrincipal("other", "other");
        TestHelpers.SetControllerContext(controller, otherUser);

        // Act
        IActionResult result = await controller.MarkAsRead(notif.Id);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }
}


