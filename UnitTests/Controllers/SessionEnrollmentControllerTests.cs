using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedClassLibrary.DTOs.Sessions;

namespace UnitTests.Controllers;

[TestClass]
public class SessionEnrollmentControllerTests : ControllerTestBase {
    private (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();

        ctx.Events.Add(new Event
        {
            IdEvent = 1,
            Title = "Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.Rooms.Add(new Room { IdRoom = 1, RoomLabel = "Room", Capacity = 2, IdEvent = 1 });

        Session pastSession = new Session
        {
            IdSession = 99,
            Title = "Past Session",
            StartTime = DateTime.UtcNow.AddDays(-2),
            EndTime = DateTime.UtcNow.AddDays(-2).AddHours(1),
            Plenary = false,
            IdEvent = 1,
            IdRoom = 1
        };
        Session futureSession = new Session
        {
            IdSession = 100,
            Title = "Future Session",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(1),
            Plenary = false,
            IdEvent = 1,
            IdRoom = 1
        };
        Session fullSession = new Session
        {
            IdSession = 101,
            Title = "Full Session",
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
            Plenary = false,
            IdEvent = 1,
            IdRoom = 1
        };
        ctx.Sessions.AddRange(pastSession, futureSession, fullSession);
        ctx.SaveChanges();

        User userA = TestHelpers.CreateTestUser("usera", "usera", "a@t.com");
        User userB = TestHelpers.CreateTestUser("userb", "userb", "b@t.com");
        ctx.Users.AddRange(userA, userB);
        ctx.SaveChanges();

        ctx.User_has_Sessions.AddRange(
            new User_has_Session { IdUser = "usera", IdSession = 101, InWaitingList = false, JoinedDate = DateTime.UtcNow },
            new User_has_Session { IdUser = "userb", IdSession = 101, InWaitingList = false, JoinedDate = DateTime.UtcNow }
        );
        ctx.SaveChanges();

        return (ctx, MockLogger<SessionEnrollmentController>());
    }

    [TestMethod]
    public async Task GetSessionData_ValidSession_ReturnsSession()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("user1", "testuser", "t@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<SessionDTO>> result = await controller.GetSessionData(100);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<SessionDTO>? response = okResult.Value as ApiResponse<SessionDTO>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual("Future Session", response.Data!.Title);
    }

    [TestMethod]
    public async Task GetSessionData_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("user1", "testuser", "t@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<SessionDTO>> result = await controller.GetSessionData(999);

        // Assert
        ObjectResult? notFound = result.Result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Enroll_ValidSession_ReturnsCreated()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("enroller", "enroller", "e@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "enroller");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("enroller", "enroller");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Enroll(100);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.IsNotNull(ctx.User_has_Sessions.FirstOrDefault(r => r.IdUser == "enroller" && r.IdSession == 100));
    }

    [TestMethod]
    public async Task Enroll_SessionNotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("enroller2", "enroller2", "e2@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "enroller2");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("enroller2", "enroller2");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Enroll(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Enroll_PastSession_ReturnsBadRequest()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("enroller3", "enroller3", "e3@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "enroller3");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("enroller3", "enroller3");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Enroll(99);

        // Assert
        ObjectResult? badRequest = result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Enroll_FullSession_PlacesInQueue()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("enroller4", "enroller4", "e4@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "enroller4");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("enroller4", "enroller4");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Enroll(101);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        User_has_Session reg = await ctx.User_has_Sessions.FirstAsync(r => r.IdUser == "enroller4" && r.IdSession == 101);
        Assert.IsTrue(reg.InWaitingList);
    }

    [TestMethod]
    public async Task Enroll_AlreadyEnrolled_ReturnsBadRequest()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("enroller5", "enroller5", "e5@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "enroller5");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = "enroller5",
            IdSession = 100,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow
        });
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("enroller5", "enroller5");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Enroll(100);

        // Assert
        ObjectResult? badRequest = result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Withdraw_ExistingRegistration_ReturnsOk()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("withdrawer", "withdrawer", "w@t.com");
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "withdrawer");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = "withdrawer",
            IdSession = 100,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow
        });
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("withdrawer", "withdrawer");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Withdraw(100);

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNull(ctx.User_has_Sessions.FirstOrDefault(r => r.IdUser == "withdrawer" && r.IdSession == 100));
    }

    [TestMethod]
    public async Task Withdraw_NoRegistration_ReturnsBadRequest()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionEnrollmentController>> loggerMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("withdrawer2", "withdrawer2", "w2@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "withdrawer2");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionEnrollmentController controller = new SessionEnrollmentController(ctx, loggerMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("withdrawer2", "withdrawer2");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Withdraw(100);

        // Assert
        ObjectResult? badRequest = result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }
}


