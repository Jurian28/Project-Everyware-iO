using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Polls;

namespace UnitTests.Controllers;

[TestClass]
public class PollControllerTests : ControllerTestBase {
    private ApplicationDbContext CreateContext()
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
    public async Task GetAllBySession_ReturnsPolls()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Poll poll = new Poll
        {
            IdPoll = 1,
            Title = "Question?",
            IdSession = 100,
            Answers = new List<PollAnswer>
            {
                new() { IdPollAnswer = 10, Text = "A" },
                new() { IdPollAnswer = 11, Text = "B" }
            }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "user");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<IEnumerable<PollDTO>>> result = await controller.GetAllBySession(100);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<PollDTO>>? response = okResult.Value as ApiResponse<IEnumerable<PollDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.Data!.Count());
    }

    [TestMethod]
    public async Task Create_ValidData_ReturnsCreated()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("speaker1", "speaker", "Speaker");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);
        PollCreateDTO dto = new PollCreateDTO
        {
            Title = "New Poll",
            IdSession = 100,
            Answers = new List<string> { "Option 1", "Option 2" }
        };

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Create(dto);

        // Assert
        ObjectResult? statusResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.AreEqual(1, await ctx.Polls.CountAsync());
    }

    [TestMethod]
    public async Task Create_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        PollController controller = new PollController(ctx);
        controller.ModelState.AddModelError("Title", "Required");

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Create(new PollCreateDTO { Title = "", IdSession = 100, Answers = new List<string>() });

        // Assert
        BadRequestObjectResult? badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task Close_ExistingPoll_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Poll poll = new Poll
        {
            IdPoll = 5,
            Title = "Close me",
            IsClosed = false,
            IdSession = 100,
            Answers = new List<PollAnswer> { new() { IdPollAnswer = 20, Text = "A", Votes = new List<PollVote>() } }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("speaker1", "speaker", "Speaker");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Close(5);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue((await ctx.Polls.FindAsync(5))!.IsClosed);
    }

    [TestMethod]
    public async Task Close_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("speaker1", "speaker", "Speaker");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Close(999);

        // Assert
        ObjectResult? notFound = result.Result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Delete_Existing_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        ctx.Polls.Add(new Poll { IdPoll = 10, Title = "Del", IdSession = 100 });
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("speaker1", "speaker", "Speaker");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Delete(10);

        // Assert
        ObjectResult? okResult = result as ObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsNull(await ctx.Polls.FindAsync(10));
    }

    [TestMethod]
    public async Task Delete_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("speaker1", "speaker", "Speaker");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);

        // Act
        IActionResult? result = await controller.Delete(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Vote_Valid_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        User user = TestHelpers.CreateTestUser("voter1", "voter", "v@t.com");
        ctx.Users.Add(user);
        Poll poll = new Poll
        {
            IdPoll = 20,
            Title = "Vote?",
            IdSession = 100,
            IsClosed = false,
            Answers = new List<PollAnswer>
            {
                new() { IdPollAnswer = 30, Text = "A", Votes = new List<PollVote>() },
                new() { IdPollAnswer = 31, Text = "B", Votes = new List<PollVote>() }
            }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("voter1", "voter");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);
        PollVoteDTO dto = new PollVoteDTO { IdPollAnswer = 30 };

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Vote(20, dto);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(1, await ctx.PollVotes.CountAsync());
    }

    [TestMethod]
    public async Task Vote_PollClosed_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Poll poll = new Poll
        {
            IdPoll = 25,
            Title = "Closed",
            IdSession = 100,
            IsClosed = true,
            Answers = new List<PollAnswer> { new() { IdPollAnswer = 35, Text = "A", Votes = new List<PollVote>() } }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("voter1", "voter");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);
        PollVoteDTO dto = new PollVoteDTO { IdPollAnswer = 35 };

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Vote(25, dto);

        // Assert
        ObjectResult? badRequest = result.Result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Vote_AlreadyVoted_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        User user = TestHelpers.CreateTestUser("voter2", "voter2", "v2@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        Poll poll = new Poll
        {
            IdPoll = 30,
            Title = "Already",
            IdSession = 100,
            IsClosed = false,
            Answers = new List<PollAnswer>
            {
                new() { IdPollAnswer = 40, Text = "A", Votes = new List<PollVote> { new() { IdUser = "voter2", IdPoll = 30, IdPollAnswer = 40 } } },
                new() { IdPollAnswer = 41, Text = "B", Votes = new List<PollVote>() }
            }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("voter2", "voter2");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);
        PollVoteDTO dto = new PollVoteDTO { IdPollAnswer = 41 };

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Vote(30, dto);

        // Assert
        ObjectResult? badRequest = result.Result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Vote_AnswerNotFoundInPoll_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Poll poll = new Poll
        {
            IdPoll = 35,
            Title = "Missing answer",
            IdSession = 100,
            Answers = new List<PollAnswer> { new() { IdPollAnswer = 50, Text = "A", Votes = new List<PollVote>() } }
        };
        ctx.Polls.Add(poll);
        ctx.SaveChanges();

        PollController controller = new PollController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "user");
        TestHelpers.SetControllerContext(controller as ControllerBase, principal);
        PollVoteDTO dto = new PollVoteDTO { IdPollAnswer = 999 };

        // Act
        ActionResult<ApiResponse<PollDTO>> result = await controller.Vote(35, dto);

        // Assert
        ObjectResult? badRequest = result.Result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }
}


