using DatabaseApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Models.Seeders;

/// <summary>
/// Seeds the database with initial demo data for events, rooms, speakers, sessions, tags, users, and enrollments.
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Runs all seed operations in the correct dependency order.
    /// </summary>
    public async Task SeedAsync()
    {
        await SeedEventsAsync();
        await SeedRoomsAsync();
        await SeedSpeakersAsync();
        await SeedSessionsAsync();
        await SeedTagsAsync();
        await SeedUsersAsync();
        await SeedUserRolesAsync();
        await SeedRelationsAsync();
    }

    private async Task SeedEventsAsync()
    {
        if (_context.Events.Any()) return;

        _context.Events.AddRange(
            new Event
            {
                Title = "iO Tech Summit 2026",
                Description = "Een dag vol inspirerende sessies over cloud, frontend, security en AI — voor en door developers.",
                StartDate = new DateTime(2026, 10, 10, 9, 0, 0),
                EndDate = new DateTime(2026, 10, 10, 17, 30, 0),
                MainColorHex = "#1E293B",
                AccentColorHex = "#3B82F6",
                Location = "'s-Hertogenbosch",
                LogoPath = "",
                OrganiserId = ""
            },
            new Event
            {
                Title = "iO Design Days 2026",
                Description = "Twee dagen UX, design systems en productontwerp met de beste designers van de Benelux.",
                StartDate = new DateTime(2026, 11, 20, 9, 0, 0),
                EndDate = new DateTime(2026, 11, 21, 17, 0, 0),
                MainColorHex = "#7C3AED",
                AccentColorHex = "#F59E0B",
                Location = "Amsterdam",
                LogoPath = "",
                OrganiserId = ""
            }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedRoomsAsync()
    {
        if (_context.Rooms.Any()) return;

        Event event1 = await _context.Events.FirstAsync(e => e.Title == "iO Tech Summit 2026");
        Event event2 = await _context.Events.FirstAsync(e => e.Title == "iO Design Days 2026");

        _context.Rooms.AddRange(
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Hoofdzaal", Description = "Grote plenaire conferentiezaal.", Capacity = 100 },
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Workshop Room A", Description = "Workshopruimte voor hands-on sessies.", Capacity = 30 },
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Workshop Room B", Description = "Workshopruimte voor kleinere groepen.", Capacity = 30 },
            new Room { IdEvent = event1.IdEvent, RoomLabel = "Innovation Lab", Description = "Kleine ruimte voor intieme deep-dives.", Capacity = 3 },
            new Room { IdEvent = event2.IdEvent, RoomLabel = "Design Studio", Description = "Creatieve werkruimte.", Capacity = 50 },
            new Room { IdEvent = event2.IdEvent, RoomLabel = "Concept Room", Description = "Ruimte voor conceptontwikkeling.", Capacity = 20 }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedSpeakersAsync()
    {
        if (_context.Speakers.Any()) return;

        Event event1 = await _context.Events.FirstAsync(e => e.Title == "iO Tech Summit 2026");
        Event event2 = await _context.Events.FirstAsync(e => e.Title == "iO Design Days 2026");

        _context.Speakers.AddRange(
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Jan", LastName = "Smit", Description = "Cloud architect met 15 jaar .NET ervaring.", ImgPath = "" },
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Emma", LastName = "de Vries", Description = "DevOps engineer en Azure-specialist.", ImgPath = "" },
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Lars", LastName = "Bergman", Description = "React Native lead bij iO digital.", ImgPath = "" },
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Sara", LastName = "Akkermans", Description = "Security consultant en OWASP-bijdrager.", ImgPath = "" },
            new Speaker { IdEvent = event1.IdEvent, FirstName = "Pieter", LastName = "van den Berg", Description = "AI-researcher en ML-engineer.", ImgPath = "" },
            new Speaker { IdEvent = event2.IdEvent, FirstName = "Nina", LastName = "Verstegen", Description = "UX director met focus op toegankelijkheid.", ImgPath = "" },
            new Speaker { IdEvent = event2.IdEvent, FirstName = "Tom", LastName = "Claes", Description = "Design systems lead bij grote retailers.", ImgPath = "" }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedSessionsAsync()
    {
        if (_context.Sessions.Any()) return;

        Event event1 = await _context.Events.FirstAsync(e => e.Title == "iO Tech Summit 2026");
        Event event2 = await _context.Events.FirstAsync(e => e.Title == "iO Design Days 2026");
        List<Room> rooms = await _context.Rooms.ToListAsync();

        Room hoofdzaal    = rooms.First(r => r.RoomLabel == "Hoofdzaal");
        Room workshopA    = rooms.First(r => r.RoomLabel == "Workshop Room A");
        Room workshopB    = rooms.First(r => r.RoomLabel == "Workshop Room B");
        Room innovLab     = rooms.First(r => r.RoomLabel == "Innovation Lab");
        Room designStudio = rooms.First(r => r.RoomLabel == "Design Studio");
        Room conceptRoom  = rooms.First(r => r.RoomLabel == "Concept Room");

        _context.Sessions.AddRange(
            // Event 1 — iO Tech Summit
            new Session { IdEvent = event1.IdEvent, IdRoom = hoofdzaal.IdRoom,  Title = "Opening Keynote: The Future of Software",         StartTime = new DateTime(2026, 10, 10, 9, 0, 0),  EndTime = new DateTime(2026, 10, 10, 10, 0, 0),  Plenary = true },
            new Session { IdEvent = event1.IdEvent, IdRoom = workshopA.IdRoom,  Title = "Cloud-native .NET met Azure Container Apps",      StartTime = new DateTime(2026, 10, 10, 10, 30, 0), EndTime = new DateTime(2026, 10, 10, 11, 30, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = workshopB.IdRoom,  Title = "React Native: van prototype naar productie",      StartTime = new DateTime(2026, 10, 10, 10, 30, 0), EndTime = new DateTime(2026, 10, 10, 11, 30, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = innovLab.IdRoom,   Title = "Zero Trust Security deep-dive",                   StartTime = new DateTime(2026, 10, 10, 11, 45, 0), EndTime = new DateTime(2026, 10, 10, 12, 45, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = workshopA.IdRoom,  Title = "AI-Powered Development met GitHub Copilot",       StartTime = new DateTime(2026, 10, 10, 13, 30, 0), EndTime = new DateTime(2026, 10, 10, 14, 30, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = workshopB.IdRoom,  Title = "CI/CD Pipelines: sneller leveren zonder risico",  StartTime = new DateTime(2026, 10, 10, 13, 30, 0), EndTime = new DateTime(2026, 10, 10, 14, 30, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = workshopA.IdRoom,  Title = "Event-Driven Architecturen in de praktijk",       StartTime = new DateTime(2026, 10, 10, 14, 45, 0), EndTime = new DateTime(2026, 10, 10, 15, 45, 0), Plenary = false },
            new Session { IdEvent = event1.IdEvent, IdRoom = hoofdzaal.IdRoom,  Title = "Closing Keynote & Q&A",                           StartTime = new DateTime(2026, 10, 10, 16, 0, 0),  EndTime = new DateTime(2026, 10, 10, 17, 30, 0), Plenary = true },

            // Event 2 — iO Design Days (dag 1)
            new Session { IdEvent = event2.IdEvent, IdRoom = designStudio.IdRoom, Title = "Keynote: Design als strategisch wapen",         StartTime = new DateTime(2026, 11, 20, 9, 0, 0),  EndTime = new DateTime(2026, 11, 20, 10, 0, 0),  Plenary = true },
            new Session { IdEvent = event2.IdEvent, IdRoom = conceptRoom.IdRoom,  Title = "Design Systems op schaal",                       StartTime = new DateTime(2026, 11, 20, 10, 30, 0), EndTime = new DateTime(2026, 11, 20, 11, 30, 0), Plenary = false },
            new Session { IdEvent = event2.IdEvent, IdRoom = designStudio.IdRoom, Title = "Inclusief ontwerpen: toegankelijkheid in UX",   StartTime = new DateTime(2026, 11, 20, 13, 0, 0),  EndTime = new DateTime(2026, 11, 20, 14, 0, 0),  Plenary = false },

            // Event 2 — iO Design Days (dag 2)
            new Session { IdEvent = event2.IdEvent, IdRoom = designStudio.IdRoom, Title = "Van idee naar clickable prototype",             StartTime = new DateTime(2026, 11, 21, 9, 30, 0),  EndTime = new DateTime(2026, 11, 21, 10, 30, 0), Plenary = false },
            new Session { IdEvent = event2.IdEvent, IdRoom = designStudio.IdRoom, Title = "Closing: Design Thinking in actie",             StartTime = new DateTime(2026, 11, 21, 16, 0, 0),  EndTime = new DateTime(2026, 11, 21, 17, 0, 0),  Plenary = true }
        );

        await _context.SaveChangesAsync();
    }

    private async Task SeedTagsAsync()
    {
        if (_context.Tags.Any()) return;

        Event event1 = await _context.Events.FirstAsync(e => e.Title == "iO Tech Summit 2026");
        Event event2 = await _context.Events.FirstAsync(e => e.Title == "iO Design Days 2026");

        Tag tagPlenair  = new Tag { IdEvent = event1.IdEvent, Title = "Plenair",  ColorHex = "#D5B82C" };
        Tag tagCloud    = new Tag { IdEvent = event1.IdEvent, Title = "Cloud",    ColorHex = "#2563EB" };
        Tag tagFrontend = new Tag { IdEvent = event1.IdEvent, Title = "Frontend", ColorHex = "#16A34A" };
        Tag tagSecurity = new Tag { IdEvent = event1.IdEvent, Title = "Security", ColorHex = "#DC2626" };
        Tag tagAI       = new Tag { IdEvent = event1.IdEvent, Title = "AI & ML",  ColorHex = "#7C3AED" };
        Tag tagDevOps   = new Tag { IdEvent = event1.IdEvent, Title = "DevOps",   ColorHex = "#EA580C" };
        Tag tagUX       = new Tag { IdEvent = event2.IdEvent, Title = "UX",       ColorHex = "#0891B2" };
        Tag tagDesign   = new Tag { IdEvent = event2.IdEvent, Title = "Design",   ColorHex = "#BE185D" };

        _context.Tags.AddRange(tagPlenair, tagCloud, tagFrontend, tagSecurity, tagAI, tagDevOps, tagUX, tagDesign);
        await _context.SaveChangesAsync();

        List<Session> sessions = await _context.Sessions.Include(s => s.Tags).ToListAsync();

        void TagSession(string title, params Tag[] tags)
        {
            Session? s = sessions.FirstOrDefault(x => x.Title == title);
            if (s != null) s.Tags = tags.ToList();
        }

        TagSession("Opening Keynote: The Future of Software",        tagPlenair);
        TagSession("Cloud-native .NET met Azure Container Apps",     tagCloud, tagDevOps);
        TagSession("React Native: van prototype naar productie",     tagFrontend);
        TagSession("Zero Trust Security deep-dive",                  tagSecurity);
        TagSession("AI-Powered Development met GitHub Copilot",      tagAI);
        TagSession("CI/CD Pipelines: sneller leveren zonder risico", tagDevOps, tagCloud);
        TagSession("Event-Driven Architecturen in de praktijk",      tagCloud);
        TagSession("Closing Keynote & Q&A",                          tagPlenair);
        TagSession("Keynote: Design als strategisch wapen",          tagDesign);
        TagSession("Design Systems op schaal",                       tagDesign, tagUX);
        TagSession("Inclusief ontwerpen: toegankelijkheid in UX",    tagUX);
        TagSession("Van idee naar clickable prototype",              tagDesign);
        TagSession("Closing: Design Thinking in actie",              tagDesign);

        await _context.SaveChangesAsync();
    }

    private async Task SeedUsersAsync()
    {
        if (_context.Users.Any()) return;

        PasswordHasher<User> hasher = new PasswordHasher<User>();

        List<User> users = new List<User>
        {
            new User { Id = "1", UserName = "jan.smit@example.com",      NormalizedUserName = "JAN.SMIT@EXAMPLE.COM",      Email = "jan.smit@example.com",      NormalizedEmail = "JAN.SMIT@EXAMPLE.COM",      EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-001" },
            new User { Id = "2", UserName = "john.doe@example.com",      NormalizedUserName = "JOHN.DOE@EXAMPLE.COM",      Email = "john.doe@example.com",      NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",      EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-002" },
            new User { Id = "3", UserName = "jane.smith@example.com",    NormalizedUserName = "JANE.SMITH@EXAMPLE.COM",    Email = "jane.smith@example.com",    NormalizedEmail = "JANE.SMITH@EXAMPLE.COM",    EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-003" },
            new User { Id = "4", UserName = "emma.devries@example.com",  NormalizedUserName = "EMMA.DEVRIES@EXAMPLE.COM",  Email = "emma.devries@example.com",  NormalizedEmail = "EMMA.DEVRIES@EXAMPLE.COM",  EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-004" },
            new User { Id = "5", UserName = "lars.bergman@example.com",  NormalizedUserName = "LARS.BERGMAN@EXAMPLE.COM",  Email = "lars.bergman@example.com",  NormalizedEmail = "LARS.BERGMAN@EXAMPLE.COM",  EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-005" },
            new User { Id = "6", UserName = "sara.akkermans@example.com",NormalizedUserName = "SARA.AKKERMANS@EXAMPLE.COM",Email = "sara.akkermans@example.com",NormalizedEmail = "SARA.AKKERMANS@EXAMPLE.COM",EmailConfirmed = true, SecurityStamp = "STATIC-STAMP-006" },
        };

        foreach (User u in users)
        {
            u.PasswordHash = hasher.HashPassword(u, "Password");
        }

        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();
    }

    private async Task SeedUserRolesAsync()
    {
        IdentityRole? adminRole   = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
        IdentityRole? speakerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Speaker");

        User? adminUser    = await _context.Users.FirstOrDefaultAsync(u => u.Email == "jan.smit@example.com");
        User? speakerUser  = await _context.Users.FirstOrDefaultAsync(u => u.Email == "emma.devries@example.com");
        User? speakerUser2 = await _context.Users.FirstOrDefaultAsync(u => u.Email == "lars.bergman@example.com");

        if (adminRole != null && adminUser != null)
        {
            bool exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
            if (!exists)
                _context.UserRoles.Add(new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = adminRole.Id });
        }

        if (speakerRole != null && speakerUser != null)
        {
            bool exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == speakerUser.Id && ur.RoleId == speakerRole.Id);
            if (!exists)
                _context.UserRoles.Add(new IdentityUserRole<string> { UserId = speakerUser.Id, RoleId = speakerRole.Id });
        }

        if (speakerRole != null && speakerUser2 != null)
        {
            bool exists = await _context.UserRoles.AnyAsync(ur => ur.UserId == speakerUser2.Id && ur.RoleId == speakerRole.Id);
            if (!exists)
                _context.UserRoles.Add(new IdentityUserRole<string> { UserId = speakerUser2.Id, RoleId = speakerRole.Id });
        }

        await _context.SaveChangesAsync();
    }

    private async Task SeedRelationsAsync()
    {
        if (_context.User_has_Sessions.Any()) return;

        Event event1 = await _context.Events.FirstAsync(e => e.Title == "iO Tech Summit 2026");
        Event event2 = await _context.Events.FirstAsync(e => e.Title == "iO Design Days 2026");

        User user1 = await _context.Users.FirstAsync(u => u.Email == "jan.smit@example.com");
        User user2 = await _context.Users.FirstAsync(u => u.Email == "john.doe@example.com");
        User user3 = await _context.Users.FirstAsync(u => u.Email == "jane.smith@example.com");
        User user4 = await _context.Users.FirstAsync(u => u.Email == "emma.devries@example.com");
        User user5 = await _context.Users.FirstAsync(u => u.Email == "lars.bergman@example.com");
        User user6 = await _context.Users.FirstAsync(u => u.Email == "sara.akkermans@example.com");

        event1.Users = new List<User> { user1, user2, user3, user4, user5, user6 };
        event2.Users = new List<User> { user1, user3, user5, user6 };

        List<Session> sessions  = await _context.Sessions.Include(s => s.Speakers).ToListAsync();
        List<Speaker> speakers  = await _context.Speakers.ToListAsync();

        void AssignSpeaker(string sessionTitle, string firstName, string lastName)
        {
            Session? s  = sessions.FirstOrDefault(x => x.Title == sessionTitle);
            Speaker? sp = speakers.FirstOrDefault(x => x.FirstName == firstName && x.LastName == lastName);
            if (s != null && sp != null) s.Speakers = new List<Speaker> { sp };
        }

        AssignSpeaker("Opening Keynote: The Future of Software",        "Jan",    "Smit");
        AssignSpeaker("Cloud-native .NET met Azure Container Apps",     "Emma",   "de Vries");
        AssignSpeaker("React Native: van prototype naar productie",     "Lars",   "Bergman");
        AssignSpeaker("Zero Trust Security deep-dive",                  "Sara",   "Akkermans");
        AssignSpeaker("AI-Powered Development met GitHub Copilot",      "Pieter", "van den Berg");
        AssignSpeaker("CI/CD Pipelines: sneller leveren zonder risico", "Emma",   "de Vries");
        AssignSpeaker("Event-Driven Architecturen in de praktijk",      "Jan",    "Smit");
        AssignSpeaker("Closing Keynote & Q&A",                         "Jan",    "Smit");
        AssignSpeaker("Keynote: Design als strategisch wapen",          "Nina",   "Verstegen");
        AssignSpeaker("Design Systems op schaal",                       "Tom",    "Claes");
        AssignSpeaker("Inclusief ontwerpen: toegankelijkheid in UX",    "Nina",   "Verstegen");
        AssignSpeaker("Van idee naar clickable prototype",              "Tom",    "Claes");
        AssignSpeaker("Closing: Design Thinking in actie",              "Nina",   "Verstegen");

        await _context.SaveChangesAsync();

        Session sCloud     = sessions.First(s => s.Title == "Cloud-native .NET met Azure Container Apps");
        Session sReact     = sessions.First(s => s.Title == "React Native: van prototype naar productie");
        Session sZeroTrust = sessions.First(s => s.Title == "Zero Trust Security deep-dive");
        Session sAI        = sessions.First(s => s.Title == "AI-Powered Development met GitHub Copilot");
        Session sCiCd      = sessions.First(s => s.Title == "CI/CD Pipelines: sneller leveren zonder risico");
        Session sEvents    = sessions.First(s => s.Title == "Event-Driven Architecturen in de praktijk");

        _context.User_has_Sessions.AddRange(
            new User_has_Session { IdUser = user1.Id, IdSession = sCloud.IdSession,     InWaitingList = false, JoinedDate = new DateTime(2026, 9, 1, 10, 0, 0) },
            new User_has_Session { IdUser = user3.Id, IdSession = sCloud.IdSession,     InWaitingList = false, JoinedDate = new DateTime(2026, 9, 1, 11, 0, 0) },
            new User_has_Session { IdUser = user5.Id, IdSession = sCloud.IdSession,     InWaitingList = false, JoinedDate = new DateTime(2026, 9, 2, 9, 0, 0) },

            new User_has_Session { IdUser = user2.Id, IdSession = sReact.IdSession,     InWaitingList = false, JoinedDate = new DateTime(2026, 9, 1, 10, 0, 0) },
            new User_has_Session { IdUser = user6.Id, IdSession = sReact.IdSession,     InWaitingList = false, JoinedDate = new DateTime(2026, 9, 2, 14, 0, 0) },

            // Zero Trust — vol (capacity 3), 1 op wachtlijst
            new User_has_Session { IdUser = user1.Id, IdSession = sZeroTrust.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 9, 5, 8, 0, 0) },
            new User_has_Session { IdUser = user2.Id, IdSession = sZeroTrust.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 9, 5, 9, 0, 0) },
            new User_has_Session { IdUser = user3.Id, IdSession = sZeroTrust.IdSession, InWaitingList = false, JoinedDate = new DateTime(2026, 9, 5, 10, 0, 0) },
            new User_has_Session { IdUser = user4.Id, IdSession = sZeroTrust.IdSession, InWaitingList = true,  JoinedDate = new DateTime(2026, 9, 5, 11, 0, 0) },

            new User_has_Session { IdUser = user2.Id, IdSession = sAI.IdSession,        InWaitingList = false, JoinedDate = new DateTime(2026, 9, 3, 9, 0, 0) },
            new User_has_Session { IdUser = user4.Id, IdSession = sAI.IdSession,        InWaitingList = false, JoinedDate = new DateTime(2026, 9, 3, 10, 0, 0) },
            new User_has_Session { IdUser = user5.Id, IdSession = sAI.IdSession,        InWaitingList = false, JoinedDate = new DateTime(2026, 9, 3, 11, 0, 0) },

            new User_has_Session { IdUser = user1.Id, IdSession = sCiCd.IdSession,      InWaitingList = false, JoinedDate = new DateTime(2026, 9, 4, 8, 0, 0) },
            new User_has_Session { IdUser = user6.Id, IdSession = sCiCd.IdSession,      InWaitingList = false, JoinedDate = new DateTime(2026, 9, 4, 9, 0, 0) },

            new User_has_Session { IdUser = user3.Id, IdSession = sEvents.IdSession,    InWaitingList = false, JoinedDate = new DateTime(2026, 9, 6, 8, 0, 0) },
            new User_has_Session { IdUser = user5.Id, IdSession = sEvents.IdSession,    InWaitingList = false, JoinedDate = new DateTime(2026, 9, 6, 9, 0, 0) }
        );

        await _context.SaveChangesAsync();
    }
}
