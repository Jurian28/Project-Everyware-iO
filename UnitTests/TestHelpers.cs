using System.Security.Claims;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace UnitTests;

public static class TestHelpers
{
    public static ApplicationDbContext CreateDbContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    public static Mock<UserManager<User>> MockUserManager()
    {
        Mock<IUserStore<User>> store = new Mock<IUserStore<User>>();
#pragma warning disable CS8604, CS8625 // Passing null to params object[] args
        return new Mock<UserManager<User>>(
            store.Object,
            null, null, null, null, null, null, null, null);
#pragma warning restore CS8604, CS8625
    }

    public static ClaimsPrincipal CreateClaimsPrincipal(string userId, string userName, params string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName),
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "TestAuth");
        return new ClaimsPrincipal(identity);
    }

    public static void SetControllerContext(Microsoft.AspNetCore.Mvc.Controller controller, ClaimsPrincipal user, Dictionary<string, string?>? cookies = null)
    {
        var httpContext = new DefaultHttpContext { User = user };
        if (cookies != null)
            httpContext.Request.Cookies = new MockRequestCookieCollection(cookies);
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    public static void SetControllerContext(ControllerBase controller, ClaimsPrincipal user)
    {
        var httpContext = new DefaultHttpContext { User = user };
        controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    public static Mock<ILogger<T>> MockLogger<T>() where T : class
    {
        return new Mock<ILogger<T>>();
    }

    public static User CreateTestUser(string id, string userName, string email)
    {
        return new User
        {
            Id = id,
            UserName = userName,
            Email = email,
            HasRequestedAccess = false
        };
    }
}

public class MockRequestCookieCollection : IRequestCookieCollection
{
    private readonly Dictionary<string, string> _cookies;

    public MockRequestCookieCollection(Dictionary<string, string?> cookies)
    {
        _cookies = cookies
            .Where(kv => kv.Value != null)
            .ToDictionary(kv => kv.Key, kv => kv.Value!);
    }

    public string? this[string key] => _cookies.TryGetValue(key, out var val) ? val : null;

    public int Count => _cookies.Count;

    public ICollection<string> Keys => _cookies.Keys;

    public bool ContainsKey(string key) => _cookies.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _cookies.GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => _cookies.GetEnumerator();

#pragma warning disable CS8769, CS8600 // Nullability mismatch with interface from nullable-oblivious assembly
    bool IRequestCookieCollection.TryGetValue(string key, out string? value)
    {
        bool result = _cookies.TryGetValue(key, out string v);
        value = v;
        return result;
    }
#pragma warning restore CS8769, CS8600
}
