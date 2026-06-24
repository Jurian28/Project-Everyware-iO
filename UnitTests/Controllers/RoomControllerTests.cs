using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.DTOs;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.DTOs.Rooms;

namespace UnitTests.Controllers;

[TestClass]
public class RoomControllerTests : ControllerTestBase {
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
        ctx.Rooms.AddRange(
            new Room { IdRoom = 1, RoomLabel = "Room A", Capacity = 50, IdEvent = 1 },
            new Room { IdRoom = 2, RoomLabel = "Room B", Capacity = 100, IdEvent = 1 }
        );
        ctx.SaveChanges();
        return ctx;
    }

    [TestMethod]
    public async Task GetAllRooms_WithoutEventId_ReturnsAll()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        RoomController controller = new RoomController(ctx);

        // Act
        ActionResult<IEnumerable<RoomResponseDTO>> result = await controller.GetAllRooms(null);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<RoomResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<RoomResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Success);
        Assert.AreEqual(2, response.Data!.Count());
    }

    [TestMethod]
    public async Task GetAllRooms_WithEventId_ReturnsFiltered()
    {
        // Arrange
        using ApplicationDbContext ctx = CreateContext();
        RoomController controller = new RoomController(ctx);

        // Act
        ActionResult<IEnumerable<RoomResponseDTO>> result = await controller.GetAllRooms(1);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<IEnumerable<RoomResponseDTO>>? response = okResult.Value as ApiResponse<IEnumerable<RoomResponseDTO>>;
        Assert.IsNotNull(response);
        Assert.AreEqual(2, response.Data!.Count());
    }

    [TestMethod]
    public async Task InsertRoom_Valid_ReturnsCreated()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomInsert");
        ctx.Events.Add(new Event { IdEvent = 5, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.SaveChanges();
        RoomController controller = new RoomController(ctx);
        RoomInsertDTO dto = new RoomInsertDTO { RoomLabel = "New Room", Capacity = 30, IdEvent = 5 };

        // Act
        ActionResult<ApiResponse<RoomResponseDTO>> result = await controller.InsertRoom(dto);

        // Assert
        ObjectResult? statusResult = result.Result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.IsNotNull(ctx.Rooms.FirstOrDefault(r => r.RoomLabel == "New Room"));
    }

    [TestMethod]
    public async Task InsertRoom_InvalidModel_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomInsertInvalid");
        RoomController controller = new RoomController(ctx);
        controller.ModelState.AddModelError("RoomLabel", "Required");

        // Act
        ActionResult<ApiResponse<RoomResponseDTO>> result = await controller.InsertRoom(new RoomInsertDTO { RoomLabel = "", Capacity = 0, IdEvent = 1 });

        // Assert
        BadRequestObjectResult? badRequest = result.Result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task UpdateRoom_Existing_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomUpdate");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Rooms.Add(new Room { IdRoom = 10, RoomLabel = "Old", Capacity = 20, IdEvent = 1 });
        ctx.SaveChanges();
        RoomController controller = new RoomController(ctx);
        RoomUpdateDTO dto = new RoomUpdateDTO { RoomLabel = "Updated", Capacity = 50 };

        // Act
        ActionResult<Room> result = await controller.UpdateRoom(10, dto);

        // Assert
        OkObjectResult? okResult = result.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Room? updated = await ctx.Rooms.FindAsync(10);
        Assert.AreEqual("Updated", updated!.RoomLabel);
        Assert.AreEqual(50, updated.Capacity);
    }

    [TestMethod]
    public async Task UpdateRoom_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomUpdateNF");
        RoomController controller = new RoomController(ctx);

        // Act
        ActionResult<Room> result = await controller.UpdateRoom(999, new RoomUpdateDTO { RoomLabel = "X", Capacity = 10 });

        // Assert
        NotFoundResult? notFound = result.Result as NotFoundResult;
        Assert.IsNotNull(notFound);
    }

    [TestMethod]
    public async Task DeleteRoom_Existing_ReturnsNoContent()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomDelete");
        ctx.Events.Add(new Event { IdEvent = 1, Title = "E", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), OrganiserId = "o" });
        ctx.Rooms.Add(new Room { IdRoom = 20, RoomLabel = "Del", Capacity = 10, IdEvent = 1 });
        ctx.SaveChanges();
        RoomController controller = new RoomController(ctx);

        // Act
        IActionResult? result = await controller.DeleteRoom(20);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(204, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task DeleteRoom_NotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoomDeleteNF");
        RoomController controller = new RoomController(ctx);

        // Act
        IActionResult? result = await controller.DeleteRoom(999);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }
}


