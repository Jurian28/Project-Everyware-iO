using Microsoft.EntityFrameworkCore;
using DatabaseApi.Models;

namespace DatabaseApi.Models.Seeders;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        await SeedEventsAsync();
        await SeedRoomsAsync();
        await SeedSpeakersAsync();
        await SeedSessionsAsync();
        await SeedTagsAsync();
        await SeedUsersAsync();
        await SeedRelationsAsync();
    }

    private async Task SeedEventsAsync()
    {
        if (_context.Events.Any()) return;

        _context.Events.Add(new Event
        {
            Title = "iO Event Connect",
            Description = "Hier zal besproken worden wat er allemaal gemaakt moet worden voor de beste event calender ooit.",
            StartDate = new DateTime(2026, 10, 10, 9, 0, 0),
            EndDate = new DateTime(2026, 10, 10, 17, 0, 0),
            MainColorHex = "#D9D9D9",
            AccentColorHex = "#B0B0B0",
            Location = "'s-Hertogenbosch",
            LogoPath = ""
        });

        await _context.SaveChangesAsync();
    }

    private async Task SeedRoomsAsync()
    {
        if (_context.Rooms.Any()) return;

        var event1 = await _context.Events.FirstAsync();

        _context.Rooms.AddRange(
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Hoofdzaal", Description = "Grote conferentiezaal.", Capacity = 100 },
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Kamer 1", Description = "Grote meeting zaal.", Capacity = 20 },
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Kamer 2", Description = "Kleine meeting zaal.", Capacity = 2 }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedSpeakersAsync()
    {
        if (_context.Speakers.Any()) return;

        var event1 = await _context.Events.FirstAsync();

        _context.Speakers.AddRange(
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Jan", LastName = "Smit", Description = "Expert in C# en Cloud.", ImgPath = "" },
            new Speaker { IdEvent = event1.IdEvent, FirstName = "John", LastName = "Doe", Description = "Expert in Databases en networking.", ImgPath = "" }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedSessionsAsync()
    {
        if (_context.Sessions.Any()) return;

        var event1 = await _context.Events.FirstAsync();
        var rooms = await _context.Rooms.ToListAsync();

        _context.Sessions.AddRange(
            new Session { IdEvent = event1.IdEvent, IdRoom = rooms[0].IdRoom, Title = "Bespreking algemene eisen en wensen.", StartTime = new DateTime(2026, 10, 10, 9, 0, 0), EndTime = new DateTime(2026, 10, 10, 10, 0, 0), Plenary = true },
            new Session { IdEvent = event1.IdEvent, IdRoom = rooms[1].IdRoom, Title = "Database architectuur.", StartTime = new DateTime(2026, 10, 10, 10, 30, 0), EndTime = new DateTime(2026, 10, 10, 11, 30, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = rooms[2].IdRoom, Title = "Routing architectuur.", StartTime = new DateTime(2026, 10, 10, 10, 30, 0), EndTime = new DateTime(2026, 10, 10, 11, 30, 0), Plenary = false }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedTagsAsync()
    {
        if (_context.Tags.Any()) return;

        var event1 = await _context.Events.FirstAsync();
        var session1 = await _context.Sessions.FirstAsync();

        var tags = new List<Tag>
        {
            new Tag { IdEvent = event1.IdEvent, Title = "Plenaire sessie", ColorHex = "#D5B82C" },
            new Tag { IdEvent = event1.IdEvent, Title = "Technology", ColorHex = "#2CCFD5" }
        };

        _context.Tags.AddRange(tags);
        await _context.SaveChangesAsync();

        var tag1 = await _context.Tags.FirstAsync(t => t.Title == "Plenaire sessie");
        var tag2 = await _context.Tags.FirstAsync(t => t.Title == "Technology");

        session1.Tags = new List<Tag> { tag1, tag2 };
        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        if (_context.Users.Any()) return;

        string passwordHash = "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==";

        _context.Users.AddRange(
            new User { Id = "1", UserName = "jan.smit@example.com", NormalizedUserName = "JAN.SMIT@EXAMPLE.COM", Email = "jan.smit@example.com", NormalizedEmail = "JAN.SMIT@EXAMPLE.COM", EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-001", PasswordHash = passwordHash },
            new User { Id = "2", UserName = "john.doe@example.com", NormalizedUserName = "JOHN.DOE@EXAMPLE.COM", Email = "john.doe@example.com", NormalizedEmail = "JOHN.DOE@EXAMPLE.COM", EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-002", PasswordHash = passwordHash },
            new User { Id = "3", UserName = "jane.smith@example.com", NormalizedUserName = "JANE.SMITH@EXAMPLE.COM", Email = "jane.smith@example.com", NormalizedEmail = "JANE.SMITH@EXAMPLE.COM", EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-003", PasswordHash = passwordHash }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedRelationsAsync()
    {
        if (_context.User_has_Sessions.Any()) return;

        var event1 = await _context.Events.FirstAsync();
        var user1 = await _context.Users.FirstAsync(u => u.Email == "jan.smit@example.com");
        var user2 = await _context.Users.FirstAsync(u => u.Email == "john.doe@example.com");
        var user3 = await _context.Users.FirstAsync(u => u.Email == "jane.smith@example.com");

        event1.Users = new List<User> { user1, user2, user3 };
        await _context.SaveChangesAsync();

        var session1 = await _context.Sessions.FirstAsync(s => s.Title == "Bespreking algemene eisen en wensen.");
        var session3 = await _context.Sessions.FirstAsync(s => s.Title == "Routing architectuur.");
        var speaker1 = await _context.Speakers.FirstAsync(s => s.FirstName == "Jan");

        session1.Speakers = new List<Speaker> { speaker1 };
        await _context.SaveChangesAsync();

        _context.User_has_Sessions.AddRange(
            new User_has_Session { IdUser = user1.Id, IdSession = session1.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 10, 10, 8, 0, 0) },
            new User_has_Session { IdUser = user1.Id, IdSession = session3.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 10, 10, 8, 5, 0) },
            new User_has_Session { IdUser = user2.Id, IdSession = session3.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 10, 10, 10, 10, 0) }
        );

        await _context.SaveChangesAsync();
    }
}