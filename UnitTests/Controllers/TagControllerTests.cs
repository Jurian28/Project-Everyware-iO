using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.DTOs;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedClassLibrary.DTOs.Tags;

namespace UnitTests.Controllers;

[TestClass]
public class TagControllerTests : ControllerTestBase {
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
        ctx.Tags.AddRange(
            new Tag { IdTag = 1, Title = "Tag1", ColorHex = "#FF0000", IdEvent = 1 },
            new Tag { IdTag = 2, Title = "Tag2", ColorHex = "#00FF00", IdEvent = 1 },
            new Tag { IdTag = 3, Title = "Tag3", ColorHex = "#0000FF", IdEvent = 1 }
        );
        ctx.SaveChanges();
        return ctx;
    }

    [TestMethod]
    public async Task GetAllTags_WithoutEventId_ReturnsAll()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        TagController controller = new TagController(ctx);

        // Act
        ActionResult<ApiResponse<IEnumerable<TagResponseDTO>>> result = await controller.GetAllTags(null);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<TagResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<TagResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(3, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetAllTags_WithEventId_ReturnsFiltered()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        TagController controller = new TagController(ctx);

        // Act
        ActionResult<ApiResponse<IEnumerable<TagResponseDTO>>> result = await controller.GetAllTags(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<TagResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<TagResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.AreEqual(3, response.Data!.Count());
    }

    [TestMethod]
    public async Task InsertTag_ValidData_ReturnsCreated()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagInsertTest");
        ctx.Events.Add(new Event
        {
            IdEvent = 10,
            Title = "Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.SaveChanges();
        TagController controller = new TagController(ctx);
        TagInsertDTO dto = new TagInsertDTO { IdEvent = 10, Title = "NewTag", ColorHex = "#123456" };

        // Act
        ActionResult<ApiResponse<TagResponseDTO>> result = await controller.InsertTag(dto);

        // Assert
        ObjectResult? statusResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Tag? tagInDb = ctx.Tags.FirstOrDefault(t => t.Title == "NewTag");
        Assert.IsNotNull(tagInDb);
    }

    [TestMethod]
    public async Task InsertTag_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagInsertInvalid");
        TagController controller = new TagController(ctx);
        controller.ModelState.AddModelError("Title", "Required");

        // Act
        ActionResult<ApiResponse<TagResponseDTO>> result = await controller.InsertTag(new TagInsertDTO { IdEvent = 1, Title = "", ColorHex = "#000" });

        // Assert
        BadRequestObjectResult? badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task UpdateTag_Existing_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagUpdate");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Tags.Add(new Tag { IdTag = 5, Title = "Old", ColorHex = "#FFF", IdEvent = 1 });
        ctx.SaveChanges();
        TagController controller = new TagController(ctx);
        TagUpdateDTO dto = new TagUpdateDTO { Title = "Updated", ColorHex = "#000" };

        // Act
        ActionResult<ApiResponse<TagResponseDTO>> result = await controller.UpdateTag(5, dto);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Tag? updated = await ctx.Tags.FindAsync(5);
        Assert.AreEqual("Updated", updated!.Title);
    }

    [TestMethod]
    public async Task UpdateTag_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagUpdateNF");
        TagController controller = new TagController(ctx);

        // Act
        ActionResult<ApiResponse<TagResponseDTO>> result = await controller.UpdateTag(999, new TagUpdateDTO { Title = "X", ColorHex = "#000" });

        // Assert
        NotFoundObjectResult? notFound = result.Result as NotFoundObjectResult;
        Assert.IsNotNull(notFound);
    }

    [TestMethod]
    public async Task DeleteTag_Existing_ReturnsNoContent()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagDelete");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Tags.Add(new Tag { IdTag = 7, Title = "Del", ColorHex = "#FFF", IdEvent = 1 });
        ctx.SaveChanges();
        TagController controller = new TagController(ctx);

        // Act
        IActionResult? result = await controller.DeleteTag(7);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(204, statusResult.StatusCode);
        Assert.IsNull(await ctx.Tags.FindAsync(7));
    }

    [TestMethod]
    public async Task DeleteTag_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("TagDeleteNF");
        TagController controller = new TagController(ctx);

        // Act
        IActionResult? result = await controller.DeleteTag(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }
}


