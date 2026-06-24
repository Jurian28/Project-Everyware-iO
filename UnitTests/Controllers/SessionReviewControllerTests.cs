using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Sessions;

namespace UnitTests.Controllers;

[TestClass]
public class SessionReviewControllerTests : ControllerTestBase {
    private ApplicationDbContext CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        User user = TestHelpers.CreateTestUser("user1", "reviewer", "r@t.com");
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
        return ctx;
    }

    [TestMethod]
    public async Task GetSessionReviews_AuthorizedUser_ReturnsReviews()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        User savedUser = ctx.Users.First(u => u.Id == "user1");
        Session session = ctx.Sessions.First(s => s.IdSession == 100);
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = savedUser.Id,
            IdSession = session.IdSession,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow,
            IsAttending = true
        });
        ctx.SaveChanges();

        SessionReviewController controller = new SessionReviewController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "reviewer");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ActionResult<ApiResponse<SessionReviewsResponseDTO>> result = await controller.GetSessionReviewsForSession(100);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<SessionReviewsResponseDTO>? response = okResult.Value as ApiResponse<SessionReviewsResponseDTO>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.IsTrue(response.Data!.CanReview);
    }

    [TestMethod]
    public async Task GetSessionReviews_NotRegistered_ReturnsForbidden()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        SessionReviewController controller = new SessionReviewController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "reviewer");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ActionResult<ApiResponse<SessionReviewsResponseDTO>> result = await controller.GetSessionReviewsForSession(100);

        // Assert
        ObjectResult? statusCodeResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusCodeResult);
        Assert.AreEqual(403, statusCodeResult.StatusCode);
    }

    [TestMethod]
    public async Task ReviewSession_RegisteredAndAttending_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        User savedUser = ctx.Users.First(u => u.Id == "user1");
        Session session = ctx.Sessions.First(s => s.IdSession == 100);
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = savedUser.Id,
            IdSession = session.IdSession,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow,
            IsAttending = true
        });
        ctx.SaveChanges();

        SessionReviewController controller = new SessionReviewController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "reviewer");
        TestHelpers.SetControllerContext(controller, principal);
        SessionReviewDTO dto = new SessionReviewDTO { Rating = 5, Comment = "Great!" };

        // Act
        ActionResult<ApiResponse<SessionReviewDTO>> result = await controller.ReviewSession(100, dto);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(1, ctx.SessionReviews.Count());
    }

    [TestMethod]
    public async Task ReviewSession_NotAttending_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        User savedUser = ctx.Users.First(u => u.Id == "user1");
        Session session = ctx.Sessions.First(s => s.IdSession == 100);
        ctx.User_has_Sessions.Add(new User_has_Session
        {
            IdUser = savedUser.Id,
            IdSession = session.IdSession,
            InWaitingList = false,
            JoinedDate = DateTime.UtcNow,
            IsAttending = false
        });
        ctx.SaveChanges();

        SessionReviewController controller = new SessionReviewController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "reviewer");
        TestHelpers.SetControllerContext(controller, principal);
        SessionReviewDTO dto = new SessionReviewDTO { Rating = 3, Comment = "OK" };

        // Act
        ActionResult<ApiResponse<SessionReviewDTO>> result = await controller.ReviewSession(100, dto);

        // Assert
        ObjectResult? badRequest = result.Result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task ReviewSession_SessionNotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        SessionReviewController controller = new SessionReviewController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "reviewer");
        TestHelpers.SetControllerContext(controller, principal);
        SessionReviewDTO dto = new SessionReviewDTO { Rating = 4, Comment = "Nice" };

        // Act
        ActionResult<ApiResponse<SessionReviewDTO>> result = await controller.ReviewSession(999, dto);

        // Assert
        ObjectResult? notFound = result.Result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task ReviewSession_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        SessionReviewController controller = new SessionReviewController(ctx);
        controller.ModelState.AddModelError("Rating", "Required");

        // Act
        ActionResult<ApiResponse<SessionReviewDTO>> result = await controller.ReviewSession(100, new SessionReviewDTO { Rating = 0, Comment = "" });

        // Assert
        ObjectResult? badRequest = result.Result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }
}


