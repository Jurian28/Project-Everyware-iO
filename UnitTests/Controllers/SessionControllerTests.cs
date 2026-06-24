using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using DatabaseApi.Services;
using DatabaseApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SharedClassLibrary.DTOs.Sessions;

namespace UnitTests.Controllers;

[TestClass]
public class SessionControllerTests : ControllerTestBase {
    private (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> sessionServiceMock) CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        Mock<ILogger<SessionController>> loggerMock = MockLogger<SessionController>();
        Mock<ISessionService> sessionServiceMock = new Mock<ISessionService>();

        ctx.Events.Add(new Event
        {
            IdEvent = 1,
            Title = "Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.Rooms.Add(new Room { IdRoom = 1, RoomLabel = "Room A", Capacity = 50, IdEvent = 1 });
        ctx.Speakers.Add(new Speaker { IdSpeaker = 1, FirstName = "John", LastName = "Doe", IdEvent = 1 });
        ctx.Tags.Add(new Tag { IdTag = 1, Title = "Tag1", ColorHex = "#FFF", IdEvent = 1 });
        ctx.Sessions.Add(new Session
        {
            IdSession = 100,
            Title = "Session 1",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Plenary = false,
            IdEvent = 1,
            IdRoom = 1
        });
        ctx.SaveChanges();
        return (ctx, loggerMock, sessionServiceMock);
    }

    [TestMethod]
    public async Task GetAllSessions_ReturnsSessions()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());

        // Act
        ActionResult<ApiResponse<IEnumerable<SessionDTO>>> result = await controller.GetAllSessions(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<SessionDTO>>? response = okResult.Value as ApiResponse<IEnumerable<SessionDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetAllSessions_WithUserData_ReturnsEnriched()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        User user = TestHelpers.CreateTestUser("user1", "testuser", "t@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "user1");
        savedUser.Events.Add(ctx.Events.First(e => e.IdEvent == 1));
        ctx.SaveChanges();

        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<IEnumerable<SessionDTO>>> result = await controller.GetAllSessionsWithUserData(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<SessionDTO>>? response = okResult.Value as ApiResponse<IEnumerable<SessionDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetAllSessionsWithUserData_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());
        ClaimsPrincipal emptyPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        TestHelpers.SetControllerContext(controller as ControllerBase, emptyPrincipal);

        // Act
        ActionResult<ApiResponse<IEnumerable<SessionDTO>>> result = await controller.GetAllSessionsWithUserData(1);

        // Assert
        UnauthorizedResult? unauthorized = result.Result as UnauthorizedResult;
        Assert.IsNotNull(unauthorized);
    }

    [TestMethod]
    public async Task GetAddSessionData_ReturnsFormOptions()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());

        // Act
        ActionResult<ApiResponse<CUSessionDTO>> result = await controller.GetAddSessionData(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<CUSessionDTO>? response = okResult.Value as ApiResponse<CUSessionDTO>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.Data!.AvailableRooms.Count);
        Assert.AreEqual(1, response.Data.AvailableTags.Count);
        Assert.AreEqual(1, response.Data.AvailableSpeakers.Count);
    }

    [TestMethod]
    public async Task GetEditSessionData_ReturnsFormOptionsWithSession()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());

        // Act
        ActionResult<ApiResponse<CUSessionDTO>> result = await controller.GetEditSessionData(1, 100);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<CUSessionDTO>? response = okResult.Value as ApiResponse<CUSessionDTO>;
        Assert.IsNotNull(response);
        Assert.IsNotNull(response.Data!.session);
        Assert.AreEqual("Session 1", response.Data.session.Title);
    }

    [TestMethod]
    public async Task SaveSession_CreateNew_ReturnsCreated()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());
        SessionDTO dto = new SessionDTO
        {
            SessionId = 0,
            Title = "New Session",
            StartTime = DateTime.UtcNow.AddDays(1).AddHours(9),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(10),
            Plenary = false,
            IdRoom = 1,
            Tags = new List<SharedClassLibrary.DTOs.Tags.TagResponseDTO>()
        };

        // Act
        IActionResult? result = await controller.SaveSession(1, dto);

        // Assert
        StatusCodeResult? statusResult = result as StatusCodeResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.AreEqual(2, await ctx.Sessions.CountAsync());
    }

    [TestMethod]
    public async Task SaveSession_UpdateExisting_ReturnsCreated()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());
        SessionDTO dto = new SessionDTO
        {
            SessionId = 100,
            Title = "Updated Session",
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Plenary = true,
            IdRoom = 1,
            Tags = new List<SharedClassLibrary.DTOs.Tags.TagResponseDTO>()
        };

        // Act
        IActionResult? result = await controller.SaveSession(1, dto);

        // Assert
        StatusCodeResult? statusResult = result as StatusCodeResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.AreEqual("Updated Session", (await ctx.Sessions.FindAsync(100))!.Title);
        Assert.IsTrue((await ctx.Sessions.FindAsync(100))!.Plenary);
    }

    [TestMethod]
    public async Task SaveSession_UpdateNotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());
        SessionDTO dto = new SessionDTO
        {
            SessionId = 999,
            Title = "Ghost",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Plenary = false,
            IdRoom = 1,
            Tags = new List<SharedClassLibrary.DTOs.Tags.TagResponseDTO>()
        };

        // Act
        IActionResult? result = await controller.SaveSession(1, dto);

        // Assert
        NotFoundObjectResult? notFound = result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
    }

    [TestMethod]
    public async Task DeleteSession_Existing_ReturnsNoContent()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());

        // Act
        IActionResult? result = await controller.DeleteSession(1, 100);

        // Assert
        StatusCodeResult? statusResult = result as StatusCodeResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(204, statusResult.StatusCode);
        Assert.IsNull(await ctx.Sessions.FindAsync(100));
    }

    [TestMethod]
    public async Task DeleteSession_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> _) = CreateContext();
        SessionController controller = new SessionController(ctx, loggerMock.Object, Mock.Of<ISessionService>());

        // Act
        IActionResult? result = await controller.DeleteSession(1, 999);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task GetSpotsData_ReturnsSpots()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<ILogger<SessionController>> loggerMock, Mock<ISessionService> sessionServiceMock) = CreateContext();
        List<SessionSpotsDTO> expected = new List<SessionSpotsDTO>
        {
            new() { SessionId = 100, SessionTitle = "S1", StartTime = DateTime.UtcNow, TotalSpots = 50, FilledSpots = 0, SpotsInWaitingList = 0, IsPlenarySession = false, Attendees = new List<AttendeeDTO>() }
        };
        sessionServiceMock.Setup(s => s.ReturnSessionSpotsData(1, null))
            .ReturnsAsync(expected);

        SessionController controller = new SessionController(ctx, loggerMock.Object, sessionServiceMock.Object);

        // Act
        ApiResponse<List<SessionSpotsDTO>> result = await controller.GetSpotsData(1, null);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data!.Count);
    }
}


