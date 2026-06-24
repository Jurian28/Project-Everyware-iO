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
public class AuthControllerTests : ControllerTestBase {
    [TestMethod]
    public async Task Login_ValidCredentials_ReturnsTokens()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLogin");
        User user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.CheckPasswordAsync(user, "password123"))
            .ReturnsAsync(true);
        userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        // Need to set environment variable for JWT secret
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        IActionResult? result = await controller.Login(new AuthInputDto { Email = "test@test.com", Password = "password123" });

        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        object? tokenResponse = okResult.Value;
        Assert.IsNotNull(tokenResponse);

        Dictionary<string, object?> dict = tokenResponse.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(tokenResponse));
        Assert.IsNotNull(dict["AccessToken"]);
        Assert.IsNotNull(dict["RefreshToken"]);
    }

    [TestMethod]
    public async Task Login_MissingCredentials_ReturnsBadRequest()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLoginNoCreds");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Login(new AuthInputDto { Email = "", Password = "" });

        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLoginBadEmail");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("none@test.com"))
            .ReturnsAsync((User?)null);

        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Login(new AuthInputDto { Email = "none@test.com", Password = "pass" });

        UnauthorizedObjectResult? unauthorized = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(401, unauthorized.StatusCode);
    }

    [TestMethod]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLoginWrongPwd");
        User user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.CheckPasswordAsync(user, "wrong"))
            .ReturnsAsync(false);

        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Login(new AuthInputDto { Email = "test@test.com", Password = "wrong" });

        UnauthorizedObjectResult? unauthorized = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(401, unauthorized.StatusCode);
    }

    [TestMethod]
    public async Task Register_ValidData_ReturnsCreated()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRegister");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("new@test.com"))
            .ReturnsAsync((User?)null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), "password123"))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>());

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Register(new AuthInputDto { Email = "new@test.com", Password = "password123" });

        ObjectResult? statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRegisterExisting");
        User user = TestHelpers.CreateTestUser("user1", "existing@test.com", "existing@test.com");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("existing@test.com"))
            .ReturnsAsync(user);

        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Register(new AuthInputDto { Email = "existing@test.com", Password = "pass" });

        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Register_FailedCreation_ReturnsBadRequest()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRegisterFail");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("fail@test.com"))
            .ReturnsAsync((User?)null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), "pass"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Register(new AuthInputDto { Email = "fail@test.com", Password = "pass" });

        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Register_MissingCredentials_ReturnsBadRequest()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRegisterNoCreds");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);

        IActionResult? result = await controller.Register(new AuthInputDto { Email = "", Password = "" });

        BadRequestObjectResult? badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Logout_WithValidCookie_RemovesToken()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLogout");
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "valid-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(1)
        });
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        Dictionary<string, string?> cookies = new Dictionary<string, string?> { ["RefreshToken"] = "valid-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        IActionResult? result = await controller.Logout();

        Assert.IsInstanceOfType(result, typeof(OkResult));
        Assert.AreEqual(0, await ctx.RefreshTokens.CountAsync());
    }

    [TestMethod]
    public async Task Logout_WithoutCookie_ReturnsOk()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthLogoutNoCookie");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller, principal);

        IActionResult? result = await controller.Logout();

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public async Task Refresh_WithValidToken_ReturnsNewTokens()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRefresh");
        User user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        ctx.Users.Add(user);
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "valid-refresh-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(1)
        });
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user1"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        Dictionary<string, string?> cookies = new Dictionary<string, string?> { ["RefreshToken"] = "valid-refresh-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        IActionResult? result = await controller.Refresh();

        OkObjectResult? okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(ctx.RefreshTokens.Count() == 1); // old removed, new added
    }

    [TestMethod]
    public async Task Refresh_WithoutCookie_ReturnsUnauthorized()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRefreshNoCookie");
        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        TestHelpers.SetControllerContext(controller, new ClaimsPrincipal(new ClaimsIdentity()), new Dictionary<string, string?>());

        IActionResult? result = await controller.Refresh();

        UnauthorizedResult? unauthorized = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorized);
    }

    [TestMethod]
    public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
    {
        using ApplicationDbContext ctx = TestHelpers.CreateDbContext("AuthRefreshExpired");
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(-1)
        });
        ctx.SaveChanges();

        Mock<UserManager<User>> userManagerMock = TestHelpers.MockUserManager();
        AuthController controller = new AuthController(userManagerMock.Object, ctx);
        ClaimsPrincipal principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        Dictionary<string, string?> cookies = new Dictionary<string, string?> { ["RefreshToken"] = "expired-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        IActionResult? result = await controller.Refresh();

        UnauthorizedResult? unauthorized = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorized);
    }
}


