using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UnitTests.Controllers;

[TestClass]
public class EventInviteControllerTests : ControllerTestBase {
    [TestMethod]
    public async Task CreateInvite_ValidData_ReturnsCreated()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteCreate");
        ctx.Events.Add(new Event
        {
            IdEvent = 1,
            Title = "Test Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.SaveChanges();
        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        EventInviteCreateDto dto = new EventInviteCreateDto { EventId = 1, Expires = DateTime.UtcNow.AddDays(7) };

        // Act
        IActionResult? result = await controller.CreateInvite(dto);

        // Assert
        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
        Assert.AreEqual(1, await ctx.EventInvites.CountAsync());
    }

    [TestMethod]
    public async Task CreateInvite_EventNotFound_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteNoEvent");
        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        EventInviteCreateDto dto = new EventInviteCreateDto { EventId = 999, Expires = DateTime.UtcNow.AddDays(7) };

        // Act
        IActionResult? result = await controller.CreateInvite(dto);

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task CreateInvite_PastExpiry_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteExpired");
        ctx.Events.Add(new Event
        {
            IdEvent = 2,
            Title = "E",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        });
        ctx.SaveChanges();
        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("org1", "organiser", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        EventInviteCreateDto dto = new EventInviteCreateDto { EventId = 2, Expires = DateTime.UtcNow.AddDays(-1) };

        // Act
        IActionResult? result = await controller.CreateInvite(dto);

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task AcceptInvite_ValidToken_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteAccept");
        Event evt = new Event
        {
            IdEvent = 10,
            Title = "Event",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        };
        ctx.Events.Add(evt);
        ctx.EventInvites.Add(new EventInvite
        {
            Token = "ABCD-1234",
            Event = evt,
            Expires = DateTime.UtcNow.AddDays(7)
        });
        User user = TestHelpers.CreateTestUser("user1", "acceptor", "a@t.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "acceptor");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.AcceptInvite("ABCD-1234");

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        User savedUser = ctx.Users.Include(u => u.Events).First(u => u.Id == "user1");
        Assert.AreEqual(1, savedUser.Events.Count);
    }

    [TestMethod]
    public async Task AcceptInvite_InvalidToken_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteInvalid");
        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "user");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.AcceptInvite("INVALID");

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task AcceptInvite_ExpiredToken_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("InviteExpiredToken");
        Event evt = new Event
        {
            IdEvent = 20,
            Title = "E",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            OrganiserId = "org1"
        };
        ctx.Events.Add(evt);
        ctx.EventInvites.Add(new EventInvite
        {
            Token = "EXPI-RE01",
            Event = evt,
            Expires = DateTime.UtcNow.AddDays(-1)
        });
        ctx.SaveChanges();

        EventInviteController controller = new EventInviteController(ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "user");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.AcceptInvite("EXPI-RE01");

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(0, await ctx.EventInvites.CountAsync());
    }
}


