using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.DTOs;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SharedClassLibrary.DTOs.Events;

namespace UnitTests.Controllers;

[TestClass]
public class EventControllerTests : ControllerTestBase {
    private (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());

        ctx.Events.AddRange(
            new Event
            {
                IdEvent = 1,
                Title = "Event A",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                Location = "Loc A",
                OrganiserId = "org1",
                IsPublished = true
            },
            new Event
            {
                IdEvent = 2,
                Title = "Event B",
                StartDate = DateTime.UtcNow.AddDays(3),
                EndDate = DateTime.UtcNow.AddDays(4),
                Location = "Loc B",
                OrganiserId = "org1",
                IsPublished = true
            },
            new Event
            {
                IdEvent = 3,
                Title = "Event C",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(6),
                Location = "Loc C",
                OrganiserId = "org2",
                IsPublished = false
            }
        );
        ctx.SaveChanges();
        return (ctx, envMock);
    }

    [TestMethod]
    public async Task GetAll_ReturnsPagedEvents()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetAll();

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        ApiResponse<EventListDto>? response = statusResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(1, response.Data!.TotalPages);
        Assert.AreEqual(3, response.Data.Events.Count);
    }

    [TestMethod]
    public async Task GetAll_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetAll(page: 1, pageSize: 2);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        ApiResponse<EventListDto>? response = statusResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Data!.Events.Count);
        Assert.AreEqual(2, response.Data.TotalPages);
    }

    [TestMethod]
    public async Task GetAll_WithTitleFilter_ReturnsFiltered()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetAll(title: "Event A");

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        ApiResponse<EventListDto>? response = statusResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Data!.Events.Count);
        Assert.AreEqual("Event A", response.Data.Events[0].Title);
    }

    [TestMethod]
    public async Task GetById_Existing_ReturnsEvent()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetById(1);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        ApiResponse<EventDTO>? response = statusResult.Value as ApiResponse<EventDTO>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual("Event A", response.Data!.Title);
    }

    [TestMethod]
    public async Task GetById_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetById(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task GetMyEvents_AsAdmin_ReturnsAllFiltered()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.GetMyEvents();

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<EventListDto>? response = okResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(3, response.Data!.Events.Count);
    }

    [TestMethod]
    public async Task GetMyEvents_AsAdmin_WithOrganiserId_Filters()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.GetMyEvents(organiserId: "org2");

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<EventListDto>? response = okResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual(1, response.Data!.Events.Count);
        Assert.AreEqual("Event C", response.Data.Events[0].Title);
    }

    [TestMethod]
    public async Task GetMyEvents_AsOrganiser_ReturnsOwnEvents()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.GetMyEvents();

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<EventListDto>? response = okResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Data!.Events.Count);
    }

    [TestMethod]
    public async Task GetAllFromUser_ExistingUser_ReturnsEvents()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        User user = TestHelpers.CreateTestUser("userX", "userx", "x@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "userX");
        savedUser.Events.Add(ctx.Events.Find(1)!);
        savedUser.Events.Add(ctx.Events.Find(2)!);
        ctx.SaveChanges();

        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetAllFromUser("userX");

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        ApiResponse<EventListDto>? response = statusResult.Value as ApiResponse<EventListDto>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
    }

    [TestMethod]
    public async Task GetAllFromUser_NonexistentUser_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.GetAllFromUser("nobody");

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Store_ValidData_ReturnsCreated()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);
        EventCreateDto dto = new EventCreateDto
        {
            Title = "New Event",
            StartDate = DateTime.UtcNow.AddDays(10),
            EndDate = DateTime.UtcNow.AddDays(11),
            Location = "New Loc",
            MainColorHex = "#FF0000",
            AccentColorHex = "#00FF00"
        };

        // Act
        IActionResult? result = await controller.Store(dto);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.AreEqual(4, await ctx.Events.CountAsync());
    }

    [TestMethod]
    public async Task Publish_ExistingEvent_ReturnsNoContent()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        Event? evt = await ctx.Events.FindAsync(3);
        evt!.IsPublished = false;
        await ctx.SaveChangesAsync();

        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.Publish(3);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.IsTrue((await ctx.Events.FindAsync(3))!.IsPublished);
    }

    [TestMethod]
    public async Task Publish_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.Publish(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task UnPublish_ExistingEvent_ReturnsNoContent()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.UnPublish(1);

        // Assert
        Assert.IsInstanceOfType(result, typeof(NoContentResult));
        Assert.IsFalse((await ctx.Events.FindAsync(1))!.IsPublished);
    }

    [TestMethod]
    public async Task UnPublish_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.UnPublish(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Update_ValidData_ReturnsOk()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);
        EventUpdateDTO dto = new EventUpdateDTO
        {
            IdEvent = 1,
            Title = "Updated Title",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Updated Loc",
            MainColorHex = "#111111",
            AccentColorHex = "#222222"
        };

        // Act
        IActionResult? result = await controller.Update(1, dto);

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual("Updated Title", (await ctx.Events.FindAsync(1))!.Title);
    }

    [TestMethod]
    public async Task Update_NotFound_ReturnsNotFound()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);
        EventUpdateDTO dto = new EventUpdateDTO
        {
            IdEvent = 999,
            Title = "Nope",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Nowhere",
            MainColorHex = "#000",
            AccentColorHex = "#FFF"
        };

        // Act
        IActionResult? result = await controller.Update(999, dto);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task Update_UnauthorisedOwner_ReturnsForbidden()
    {
        // Arrange
        (ApplicationDbContext ctx, Mock<IWebHostEnvironment> envMock) = CreateContext();
        EventController controller = new EventController(ctx, envMock.Object);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("other", "other", "Organiser");
        TestHelpers.SetControllerContext(controller, principal);
        EventUpdateDTO dto = new EventUpdateDTO
        {
            IdEvent = 1,
            Title = "Hacked",
            StartDate = DateTime.UtcNow.AddDays(1),
            EndDate = DateTime.UtcNow.AddDays(2),
            Location = "Hmm",
            MainColorHex = "#000",
            AccentColorHex = "#FFF"
        };

        // Act
        IActionResult? result = await controller.Update(1, dto);

        // Assert
        ObjectResult? forbidden = result as ObjectResult;
        Assert.IsNotNull(forbidden);
        Assert.AreEqual(403, forbidden.StatusCode);
    }
}


