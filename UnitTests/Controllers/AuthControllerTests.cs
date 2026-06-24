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
        using var ctx = TestHelpers.CreateDbContext("AuthLogin");
        var user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        ctx.Users.Add(user);
        ctx.SaveChanges();

        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.CheckPasswordAsync(user, "password123"))
            .ReturnsAsync(true);
        userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        var controller = new AuthController(userManagerMock.Object, ctx);
        // Need to set environment variable for JWT secret
        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        var result = await controller.Login(new AuthInputDto { Email = "test@test.com", Password = "password123" });

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var tokenResponse = okResult.Value;
        Assert.IsNotNull(tokenResponse);

        var dict = tokenResponse.GetType().GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(tokenResponse));
        Assert.IsNotNull(dict["AccessToken"]);
        Assert.IsNotNull(dict["RefreshToken"]);
    }

    [TestMethod]
    public async Task Login_MissingCredentials_ReturnsBadRequest()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthLoginNoCreds");
        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Login(new AuthInputDto { Email = "", Password = "" });

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Login_InvalidEmail_ReturnsUnauthorized()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthLoginBadEmail");
        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("none@test.com"))
            .ReturnsAsync((User?)null);

        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Login(new AuthInputDto { Email = "none@test.com", Password = "pass" });

        var unauthorized = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(401, unauthorized.StatusCode);
    }

    [TestMethod]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthLoginWrongPwd");
        var user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("test@test.com"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.CheckPasswordAsync(user, "wrong"))
            .ReturnsAsync(false);

        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Login(new AuthInputDto { Email = "test@test.com", Password = "wrong" });

        var unauthorized = result as UnauthorizedObjectResult;
        Assert.IsNotNull(unauthorized);
        Assert.AreEqual(401, unauthorized.StatusCode);
    }

    [TestMethod]
    public async Task Register_ValidData_ReturnsCreated()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRegister");
        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("new@test.com"))
            .ReturnsAsync((User?)null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), "password123"))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>());

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Register(new AuthInputDto { Email = "new@test.com", Password = "password123" });

        var statusResult = result as ObjectResult;
        Assert.IsNotNull(statusResult);
        Assert.AreEqual(201, statusResult.StatusCode);
    }

    [TestMethod]
    public async Task Register_ExistingEmail_ReturnsBadRequest()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRegisterExisting");
        var user = TestHelpers.CreateTestUser("user1", "existing@test.com", "existing@test.com");
        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("existing@test.com"))
            .ReturnsAsync(user);

        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Register(new AuthInputDto { Email = "existing@test.com", Password = "pass" });

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Register_FailedCreation_ReturnsBadRequest()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRegisterFail");
        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByEmailAsync("fail@test.com"))
            .ReturnsAsync((User?)null);
        userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), "pass"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too weak" }));

        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Register(new AuthInputDto { Email = "fail@test.com", Password = "pass" });

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Register_MissingCredentials_ReturnsBadRequest()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRegisterNoCreds");
        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);

        var result = await controller.Register(new AuthInputDto { Email = "", Password = "" });

        var badRequest = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequest);
        Assert.AreEqual(400, badRequest.StatusCode);
    }

    [TestMethod]
    public async Task Logout_WithValidCookie_RemovesToken()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthLogout");
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "valid-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(1)
        });
        ctx.SaveChanges();

        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);
        var principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        var cookies = new Dictionary<string, string?> { ["RefreshToken"] = "valid-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        var result = await controller.Logout();

        Assert.IsInstanceOfType(result, typeof(OkResult));
        Assert.AreEqual(0, await ctx.RefreshTokens.CountAsync());
    }

    [TestMethod]
    public async Task Logout_WithoutCookie_ReturnsOk()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthLogoutNoCookie");
        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);
        var principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        TestHelpers.SetControllerContext(controller, principal);

        var result = await controller.Logout();

        Assert.IsInstanceOfType(result, typeof(OkResult));
    }

    [TestMethod]
    public async Task Refresh_WithValidToken_ReturnsNewTokens()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRefresh");
        var user = TestHelpers.CreateTestUser("user1", "test@test.com", "test@test.com");
        ctx.Users.Add(user);
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "valid-refresh-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(1)
        });
        ctx.SaveChanges();

        var userManagerMock = TestHelpers.MockUserManager();
        userManagerMock.Setup(um => um.FindByIdAsync("user1"))
            .ReturnsAsync(user);
        userManagerMock.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string>());

        Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "ThisIsASecretKeyForTestingPurposesOnly1234567890!");

        var controller = new AuthController(userManagerMock.Object, ctx);
        var principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        var cookies = new Dictionary<string, string?> { ["RefreshToken"] = "valid-refresh-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        var result = await controller.Refresh();

        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.IsTrue(ctx.RefreshTokens.Count() == 1); // old removed, new added
    }

    [TestMethod]
    public async Task Refresh_WithoutCookie_ReturnsUnauthorized()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRefreshNoCookie");
        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);
        TestHelpers.SetControllerContext(controller, new ClaimsPrincipal(new ClaimsIdentity()), new Dictionary<string, string?>());

        var result = await controller.Refresh();

        var unauthorized = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorized);
    }

    [TestMethod]
    public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
    {
        using var ctx = TestHelpers.CreateDbContext("AuthRefreshExpired");
        ctx.RefreshTokens.Add(new RefreshToken
        {
            Token = "expired-token",
            UserId = "user1",
            Expires = DateTime.UtcNow.AddDays(-1)
        });
        ctx.SaveChanges();

        var userManagerMock = TestHelpers.MockUserManager();
        var controller = new AuthController(userManagerMock.Object, ctx);
        var principal = TestHelpers.CreateClaimsPrincipal("user1", "testuser");
        var cookies = new Dictionary<string, string?> { ["RefreshToken"] = "expired-token" };
        TestHelpers.SetControllerContext(controller, principal, cookies);

        var result = await controller.Refresh();

        var unauthorized = result as UnauthorizedResult;
        Assert.IsNotNull(unauthorized);
    }
}


