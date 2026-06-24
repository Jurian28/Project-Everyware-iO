using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedClassLibrary.DTOs.Sessions;

namespace UnitTests.Controllers;

[TestClass]
public class AttendanceControllerTests : ControllerTestBase {
    private (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();

        User user = TestHelpers.CreateTestUser("user1", "attendee", "a@t.com");
        ctx.Users.Add(user);

        ctx.Events.Add(new Event
        {
            IdEvent = 1,
            Title = "Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.Rooms.Add(new Room { IdRoom = 1, RoomLabel = "Room", Capacity = 50, IdEvent = 1 });
        ctx.Sessions.Add(new Session
        {
            IdSession = 100,
            Title = "Session",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Plenary = false,
            IdEvent = 1,
            IdRoom = 1
        });
        ctx.SaveChanges();

        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "user1");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = savedUser.Id,
            IdSession = 100,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow,
            IsAttending = false
        });
        ctx.SaveChanges();

        return (ctx, envMock);
    }

    [TestMethod]
    public async Task CheckAttendance_Valid_ReturnsOk()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        AttendanceController controller = new AttendanceController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "attendee");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.CheckAttendance(100, "user1");

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<SessionAttendenceDTO>? response = okResult.Value as ApiResponse<SessionAttendenceDTO>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.IsTrue((await ctx.User_has_Sessions.FirstAsync()).IsAttending);
    }

    [TestMethod]
    public async Task CheckAttendance_AlreadyAttending_ReturnsConflict()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        User_has_Session reg = await ctx.User_has_Sessions.FirstAsync();
        reg.IsAttending = true;
        await ctx.SaveChangesAsync();

        AttendanceController controller = new AttendanceController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "attendee");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.CheckAttendance(100, "user1");

        // Assert
        ObjectResult? conflictResult = result as ObjectResult;
        Assert.IsNotNull(conflictResult);
        Assert.AreEqual(409, conflictResult.StatusCode);
    }

    [TestMethod]
    public async Task CheckAttendance_EmptyUserId_ReturnsUnauthorized()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        AttendanceController controller = new AttendanceController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "attendee");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.CheckAttendance(100, "");

        // Assert
        ObjectResult? unauthorizedResult = result as ObjectResult;
        Assert.IsNotNull(unauthorizedResult);
        Assert.AreEqual(401, unauthorizedResult.StatusCode);
    }

    [TestMethod]
    public async Task CheckAttendance_InvalidSessionId_ReturnsBadRequest()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        AttendanceController controller = new AttendanceController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "attendee");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.CheckAttendance(0, "user1");

        // Assert
        ObjectResult? badRequest = result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task CheckAttendance_SessionNotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        AttendanceController controller = new AttendanceController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "attendee");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.CheckAttendance(999, "user1");

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }
}


