using System.Security.Claims;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests;

public abstract class ControllerTestBase
{
    protected ApplicationDbContext CreateDbContext()
    {
        return TestHelpers.CreateDbContext();
    }

    protected ApplicationDbContext CreateDbContext(string dbName)
    {
        return TestHelpers.CreateDbContext(dbName);
    }

    protected ClaimsPrincipal CreateClaimsPrincipal(string userId, string userName, params string[] roles)
    {
        return TestHelpers.CreateClaimsPrincipal(userId, userName, roles);
    }

    protected void SetControllerContext(Microsoft.AspNetCore.Mvc.Controller controller, ClaimsPrincipal user, Dictionary<string, string?>? cookies = null)
    {
        TestHelpers.SetControllerContext(controller, user, cookies);
    }

    protected void SetControllerContext(ControllerBase controller, ClaimsPrincipal user)
    {
        TestHelpers.SetControllerContext(controller, user);
    }

    protected User CreateTestUser(string id, string userName, string email)
    {
        return TestHelpers.CreateTestUser(id, userName, email);
    }

    protected Mock<ILogger<T>> MockLogger<T>() where T : class
    {
        return TestHelpers.MockLogger<T>();
    }

    protected Mock<UserManager<User>> MockUserManager()
    {
        return TestHelpers.MockUserManager();
    }

    protected Event CreateEvent(int id, string title, string organiserId, bool published = true)
    {
        return new Event
        {
            IdEvent = id,
            Title = title,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(1),
            Location = "Location",
            OrganiserId = organiserId,
            IsPublished = published
        };
    }

    protected Room CreateRoom(int id, string label, int capacity, int eventId)
    {
        return new Room
        {
            IdRoom = id,
            RoomLabel = label,
            Capacity = capacity,
            IdEvent = eventId
        };
    }

    protected Session CreateSession(int id, string title, int eventId, int roomId, bool plenary = false)
    {
        return new Session
        {
            IdSession = id,
            Title = title,
            StartTime = DateTime.UtcNow.AddHours(1),
            EndTime = DateTime.UtcNow.AddHours(2),
            Plenary = plenary,
            IdEvent = eventId,
            IdRoom = roomId
        };
    }

    protected Speaker CreateSpeaker(int id, string firstName, string lastName, int eventId)
    {
        return new Speaker
        {
            IdSpeaker = id,
            FirstName = firstName,
            LastName = lastName,
            IdEvent = eventId
        };
    }

    protected Tag CreateTag(int id, string title, string colorHex, int eventId)
    {
        return new Tag
        {
            IdTag = id,
            Title = title,
            ColorHex = colorHex,
            IdEvent = eventId
        };
    }
}
