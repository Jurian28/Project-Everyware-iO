using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User_has_Session> User_has_Sessions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<EventInvite> EventInvites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User_has_Session relationship
            modelBuilder.Entity<User_has_Session>()
                .HasKey(uhs => new { uhs.IdUser, uhs.IdSession });

            modelBuilder.Entity<User_has_Session>()
                .HasOne(uhs => uhs.User)
                .WithMany(u => u.RegisteredSessions)
                .HasForeignKey(uhs => uhs.IdUser);

            modelBuilder.Entity<User_has_Session>()
                .HasOne(uhs => uhs.Session)
                .WithMany(s => s.RegisteredUsers)
                .HasForeignKey(uhs => uhs.IdSession);

            // User_has_Event relationship
            modelBuilder.Entity<User>()
                .HasMany(u => u.Events)
                .WithMany(s => s.Users)
                .UsingEntity("User_has_Event");

            // Room relationship with Event
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Rooms)
                .HasForeignKey(r => r.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Room>()
                .HasIndex(r => new { r.IdEvent, r.RoomLabel })
                .IsUnique();

            // Session relationship with Event
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Event)
                .WithMany()
                .HasForeignKey(s => s.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);

            // Session relationship with Room
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Sessions)
                .HasForeignKey(s => s.IdRoom);

            // Tag relationship with Event
            modelBuilder.Entity<Tag>()
                .HasOne(t => t.Event)
                .WithMany(e => e.Tags)
                .HasForeignKey(t => t.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);

            // Session_has_Tag relationship
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Tags)
                .WithMany(t => t.Sessions)
                .UsingEntity("Session_has_Tag");

            // Session_has_Speaker relationship
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Speakers)
                .WithMany(sp => sp.Sessions)
                .UsingEntity("Session_has_Speaker");

            // Speaker relationship with Event
            modelBuilder.Entity<Speaker>()
                .HasOne(s => s.Event)
                .WithMany(e => e.Speakers)
                .HasForeignKey(s => s.IdEvent)
                .OnDelete(DeleteBehavior.Restrict);

            // Seeddata
            AddSeeddata(modelBuilder);
        }

        private void AddSeeddata(ModelBuilder modelBuilder)
        {
            // Events
            modelBuilder.Entity<Event>().HasData(new Event
            {
                IdEvent = 1,
                Title = "iO Event Connect",
                Description = "Hier zal besproken worden wat er allemaal gemaakt moet worden voor de beste event calender ooit.",
                StartDate = new DateTime(2026, 10, 10, 9, 0, 0),
                EndDate = new DateTime(2026, 10, 10, 17, 0, 0),
                MainColorHex = "#D9D9D9",
                AccentColorHex = "#B0B0B0",
                Location = "'s-Hertogenbosch", 
                LogoPath = ""
            });

            // Rooms
            modelBuilder.Entity<Room>().HasData(
                new
                {
                    IdRoom = 1,
                    IdEvent = 1,
                    RoomLabel = "Hoofdzaal",
                    Description = "Grote conferentiezaal.",
                    Capacity = 100
                },
                new
                {
                    IdRoom = 2,
                    IdEvent = 1,
                    RoomLabel = "Kamer 1",
                    Description = "Grote meeting zaal.",
                    Capacity = 20
                },
                new
                {
                    IdRoom = 3,
                    IdEvent = 1,
                    RoomLabel = "Kamer 2",
                    Description = "Kleine meeting zaal.",
                    Capacity = 2
                }
            );

            // Sprekers
            modelBuilder.Entity<Speaker>().HasData(
                new
                {
                    IdSpeaker = 1,
                    IdEvent = 1,
                    FirstName = "Jan",
                    LastName = "Smit",
                    description = "Expert in C# en Cloud.",
                    ImgPath = ""
                },
                new
                {
                    IdSpeaker = 2,
                    IdEvent = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    description = "Expert in Databases en networking.",
                    ImgPath = ""
                }
            );

            // Sessions
            modelBuilder.Entity<Session>().HasData(
                new
                {
                    IdSession = 1,
                    IdEvent = 1,
                    IdRoom = 1,
                    Title = "Bespreking algemene eisen en wensen.",
                    StartTime = new DateTime(2026, 10, 10, 9, 0, 0),
                    EndTime = new DateTime(2026, 10, 10, 10, 0, 0),
                    Plenary = true
                },
                new
                {
                    IdSession = 2,
                    IdEvent = 1,
                    IdRoom = 2,
                    Title = "Database architectuur.",
                    StartTime = new DateTime(2026, 10, 10, 10, 30, 0),
                    EndTime = new DateTime(2026, 10, 10, 11, 30, 0),
                    Capacity = 20,
                    Plenary = false
                },
                new
                {
                    IdSession = 3,
                    IdEvent = 1,
                    IdRoom = 3,
                    Title = "Routing architectuur.",
                    StartTime = new DateTime(2026, 10, 10, 10, 30, 0),
                    EndTime = new DateTime(2026, 10, 10, 11, 30, 0),
                    Capacity = 2,
                    Plenary = false
                }
            );

            // Tags
            modelBuilder.Entity<Tag>().HasData(
                new { IdEvent = 1, Title = "Plenaire sessie", ColorHex = "#D5B82C" },
                new { IdEvent = 1, Title = "Technology", ColorHex = "#2CCFD5" }
            );

            // Session has tags
            modelBuilder.Entity("Session_has_Tag").HasData(
                new { SessionsIdSession = 1, TagsTitle = "Plenaire sessie", TagsIdEvent = 1 },
                new { SessionsIdSession = 1, TagsTitle = "Technology", TagsIdEvent = 1 }
            );

            // Session has speakers
            modelBuilder.Entity("Session_has_Speaker").HasData(
                new { SessionsIdSession = 1, SpeakersIdSpeaker = 1 }
            );

            modelBuilder.Entity<User>().HasData(
                new
                {
                    Id = "1",
                    UserName = "jan.smit@example.com",
                    NormalizedUserName = "JAN.SMIT@EXAMPLE.COM",
                    Email = "jan.smit@example.com",
                    NormalizedEmail = "JAN.SMIT@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = "STATIC-STAMP-001",
                    PasswordHash = "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==",
                    TwoFactorEnabled = false, /* necessary fields */
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new
                {
                    Id = "2",
                    UserName = "john.doe@example.com",
                    NormalizedUserName = "JOHN.DOE@EXAMPLE.COM",
                    Email = "john.doe@example.com",
                    NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = "STATIC-STAMP-002",
                    PasswordHash = "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==",
                    TwoFactorEnabled = false, /* necessary fields */
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                },
                new
                {
                    Id = "3",
                    UserName = "jane.smith@example.com",
                    NormalizedUserName = "JANE.SMITH@EXAMPLE.COM",
                    Email = "jane.smith@example.com",
                    NormalizedEmail = "JANE.SMITH@EXAMPLE.COM",
                    EmailConfirmed = true,
                    SecurityStamp = "STATIC-STAMP-003",
                    PasswordHash = "AQAAAAIAAYagAAAAEE9XsCMDkXMdTDw5BcaJ7teKfgRDJpxSUt6WF2/3BaCbJaJkCFImPUBAKfygEXtbVg==",
                    TwoFactorEnabled = false, /* necessary fields */
                    PhoneNumberConfirmed = false,
                    LockoutEnabled = false,
                    AccessFailedCount = 0
                }
            );

            // User has event
            modelBuilder.Entity("User_has_Event").HasData(
                new { EventsIdEvent = 1, UsersId = "1" },
                new { EventsIdEvent = 1, UsersId = "2" },
                new { EventsIdEvent = 1, UsersId = "3" }
            );

            // User has sessions
            modelBuilder.Entity<User_has_Session>().HasData(
                new
                {
                    IdUser = "1",
                    IdSession = 1,
                    InWaitingList = false,
                    JoinedDate = new DateTime(2026, 10, 10, 8, 0, 0)
                },
                new
                {
                    IdUser = "1",
                    IdSession = 3,
                    InWaitingList = false,
                    JoinedDate = new DateTime(2026, 10, 10, 8, 5, 0)
                },
                new
                {
                    IdUser = "2",
                    IdSession = 3, /* This session is now full */
                    InWaitingList = false,
                    JoinedDate = new DateTime(2026, 10, 10, 10, 10, 0)
                }
            );
        }
    }
}
