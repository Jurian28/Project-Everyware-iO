using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.DTOs.Speakers;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests.Controllers;

[TestClass]
public class SpeakerControllerTests : ControllerTestBase {
    private ApplicationDbContext CreateContext()
    {
        ApplicationDbContext ctx = TestHelpers.CreateDbContext();
        ctx.Events.Add(new Event
        {
            IdEvent = 1,
            Title = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.Speakers.AddRange(
            new Speaker { IdSpeaker = 1, FirstName = "John", LastName = "Doe", IdEvent = 1 },
            new Speaker { IdSpeaker = 2, FirstName = "Jane", LastName = "Smith", IdEvent = 1 }
        );
        ctx.SaveChanges();
        return ctx;
    }

    [TestMethod]
    public async Task GetAllSpeakers_WithoutEventId_ReturnsAll()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);

        // Act
        ActionResult<IEnumerable<SpeakerResponseDTO>> result = await controller.GetAllSpeakers(null);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<SpeakerResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<SpeakerResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(2, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetAllSpeakers_WithEventId_ReturnsFiltered()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);

        // Act
        ActionResult<IEnumerable<SpeakerResponseDTO>> result = await controller.GetAllSpeakers(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<SpeakerResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<SpeakerResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Data!.Count());
    }

    [TestMethod]
    public async Task InsertSpeaker_Valid_ReturnsCreated()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerInsert");
        ctx.Events.Add(new Event { IdEvent = 5, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.SaveChanges();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);
        SpeakerInsertDTO dto = new SpeakerInsertDTO { FirstName = "New", LastName = "Speaker", IdEvent = 5 };

        // Act
        ActionResult<ApiResponse<SpeakerResponseDTO>> result = await controller.InsertSpeaker(dto);

        // Assert
        ObjectResult? statusResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.IsNotNull(ctx.Speakers.FirstOrDefault(s => s.FirstName == "New"));
    }

    [TestMethod]
    public async Task InsertSpeaker_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerInsertInvalid");
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);
        controller.ModelState.AddModelError("FirstName", "Required");

        // Act
        ActionResult<ApiResponse<SpeakerResponseDTO>> result = await controller.InsertSpeaker(new SpeakerInsertDTO { FirstName = "", LastName = "", IdEvent = 1 });

        // Assert
        BadRequestObjectResult? badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task UpdateSpeaker_Existing_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerUpdate");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Speakers.Add(new Speaker { IdSpeaker = 10, FirstName = "Old", LastName = "Name", IdEvent = 1 });
        ctx.SaveChanges();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        envMock.Setup(e => e.WebRootPath).Returns(Path.GetTempPath());
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);
        SpeakerUpdateDTO dto = new SpeakerUpdateDTO { FirstName = "Updated", LastName = "Speaker" };

        // Act
        IActionResult? result = await controller.UpdateSpeaker(10, dto);

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Speaker? updated = await ctx.Speakers.FindAsync(10);
        Assert.AreEqual("Updated", updated!.FirstName);
    }

    [TestMethod]
    public async Task UpdateSpeaker_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerUpdateNF");
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.UpdateSpeaker(999, new SpeakerUpdateDTO { FirstName = "X", LastName = "Y" });

        // Assert
        Assert.IsInstanceOfType(result, typeof(NotFoundResult));
    }

    [TestMethod]
    public async Task DeleteSpeaker_Existing_ReturnsNoContent()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerDelete");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Speakers.Add(new Speaker { IdSpeaker = 20, FirstName = "Del", LastName = "Me", IdEvent = 1 });
        ctx.SaveChanges();
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.DeleteSpeaker(20);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(204, statusResult.StatusCode);
        Assert.IsNull(await ctx.Speakers.FindAsync(20));
    }

    [TestMethod]
    public async Task DeleteSpeaker_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("SpeakerDeleteNF");
        Mock<IWebHostEnvironment> envMock = new Mock<IWebHostEnvironment>();
        SpeakerController controller = new SpeakerController(ctx, envMock.Object);

        // Act
        IActionResult? result = await controller.DeleteSpeaker(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }
}


