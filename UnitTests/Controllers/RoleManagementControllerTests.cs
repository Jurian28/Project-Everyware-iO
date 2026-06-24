using System.Security.Claims;
using DatabaseApi.Controllers;
using DatabaseApi.Models;
using DatabaseApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests.Controllers;

[TestClass]
public class RoleManagementControllerTests : ControllerTestBase {
    [TestMethod]
    public async Task GetOrganisers_AdminUser_ReturnsOrganisers()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleGetOrgs");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        List<User> organisers = new List<User>
        {
            TestHelpers.CreateTestUser("org1", "org1@t.com", "org1@t.com"),
            TestHelpers.CreateTestUser("org2", "org2@t.com", "org2@t.com"),
        };
        userManagerMock.Setup(um => um.GetUsersInRoleAsync("Organiser"))
            .ReturnsAsync(organisers);

        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ApiResponse<object> result = await controller.GetOrganisers();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, ((IEnumerable<dynamic>)result.Data!).Count());
    }

    [TestMethod]
    public async Task GetSpeakers_AdminUser_ReturnsSpeakers()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleGetSpeakers");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        List<User> speakers = new List<User>
        {
            TestHelpers.CreateTestUser("sp1", "sp1@t.com", "sp1@t.com"),
        };
        userManagerMock.Setup(um => um.GetUsersInRoleAsync("Speaker"))
            .ReturnsAsync(speakers);

        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ApiResponse<object> result = await controller.GetSpeakers();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, ((IEnumerable<dynamic>)result.Data!).Count());
    }

    [TestMethod]
    public async Task GetOrganiserRequests_ReturnsUsersWithRequests()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleGetRequests");
        User req1 = TestHelpers.CreateTestUser("req1", "req1@t.com", "req1@t.com");
        req1.HasRequestedAccess = true;
        User req2 = TestHelpers.CreateTestUser("req2", "req2@t.com", "req2@t.com");
        req2.HasRequestedAccess = true;
        User noreq = TestHelpers.CreateTestUser("noreq", "noreq@t.com", "noreq@t.com");
        ctx.Users.AddRange(req1, req2, noreq);
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.IsInRoleAsync(It.IsAny<User>(), "Organiser"))
            .ReturnsAsync(false);

        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        ApiResponse<object> result = await controller.GetOrganiserRequests();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, ((IEnumerable<dynamic>)result.Data!).Count());
    }

    [TestMethod]
    public async Task RequestAccess_ValidUser_ReturnsOk()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRequestAccess");
        ctx.Users.Add(TestHelpers.CreateTestUser("user1", "user", "u@t.com"));
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "user");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.RequestAccess();

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue((await ctx.Users.FindAsync("user1"))!.HasRequestedAccess);
    }

    [TestMethod]
    public async Task RequestAccess_AlreadyRequested_ReturnsBadRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleAlreadyRequested");
        User user = TestHelpers.CreateTestUser("user2", "user2", "u2@t.com");
        user.HasRequestedAccess = true;
        ctx.Users.Add(user);
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user2", "user2");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.RequestAccess();

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task RequestAccess_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRequestNotFound");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("nobody", "nobody");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.RequestAccess();

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task HasRequestedAccess_UserWithRequest_ReturnsTrue()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleHasRequested");
        User user = TestHelpers.CreateTestUser("user3", "user3", "u3@t.com");
        user.HasRequestedAccess = true;
        ctx.Users.Add(user);
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user3", "user3");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.HasRequestedAccess();

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        ApiResponse<bool>? response = okResult.Value as ApiResponse<bool>;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.Data);
    }

    [TestMethod]
    public async Task RemoveRequest_SpecificUser_RemovesRequest()
    {
        // Arrange
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRemoveReq");
        User user = TestHelpers.CreateTestUser("user4", "user4", "u4@t.com");
        user.HasRequestedAccess = true;
        ctx.Users.Add(user);
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user4"))
            .ReturnsAsync(user);

        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);

        // Act
        IActionResult? result = await controller.RemoveRequest("user4");

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsFalse((await ctx.Users.FindAsync("user4"))!.HasRequestedAccess);
    }

    [TestMethod]
    public async Task InstateRole_ValidUser_ReturnsOk()
    {
        // Arrange
        User user = TestHelpers.CreateTestUser("user5", "user5", "u5@t.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user5"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.IsInRoleAsync(user, "Organiser"))
            .ReturnsAsync(false);
        userManagerMock.Setup(um => um.AddToRoleAsync(user, "Organiser"))
            .ReturnsAsync(IdentityResult.Success);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleInstate");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.InstateRole("user5", dto);

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public async Task InstateRole_AlreadyHasRole_ReturnsBadRequest()
    {
        // Arrange
        User user = TestHelpers.CreateTestUser("user6", "user6", "u6@t.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user6"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.IsInRoleAsync(user, "Organiser"))
            .ReturnsAsync(true);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleInstateExisting");
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.InstateRole("user6", dto);

        // Assert
        ObjectResult? badRequest = result as ObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task RevokeRole_ValidUser_ReturnsOk()
    {
        // Arrange
        User user = TestHelpers.CreateTestUser("user7", "user7", "u7@t.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user7"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.IsInRoleAsync(user, "Organiser"))
            .ReturnsAsync(true);
        userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "Organiser"))
            .ReturnsAsync(IdentityResult.Success);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRevoke");
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.RevokeRole("user7", dto);

        // Assert
        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
    }

    [TestMethod]
    public async Task RevokeRole_UserNotInRole_ReturnsBadRequest()
    {
        // Arrange
        User user = TestHelpers.CreateTestUser("user8", "user8", "u8@t.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user8"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.IsInRoleAsync(user, "Organiser"))
            .ReturnsAsync(false);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRevokeNotInRole");
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.RevokeRole("user8", dto);

        // Assert
        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
    }

    [TestMethod]
    public async Task InstateRole_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("nobody"))
            .ReturnsAsync((User?)null);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleInstateNF");
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.InstateRole("nobody", dto);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }

    [TestMethod]
    public async Task RevokeRole_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("nobody"))
            .ReturnsAsync((User?)null);

        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("RoleRevokeNF");
        RoleManagementController controller = new RoleManagementController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("admin1", "admin", "Admin");
        TestHelpers.SetControllerContext(controller, principal);
        RoleAssignmentDto dto = new RoleAssignmentDto { Role = "Organiser" };

        // Act
        IActionResult? result = await controller.RevokeRole("nobody", dto);

        // Assert
        ObjectResult? notFound = result as ObjectResult;
        Assert.IsNotNull(notFound);
        Assert.AreEqual(404, notFound.StatusCode);
    }
}


