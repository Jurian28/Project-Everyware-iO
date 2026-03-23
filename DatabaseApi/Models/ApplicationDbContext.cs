using Microsoft.EntityFrameworkCore;

namespace DatabaseApi.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User_has_Session> User_has_Sessions { get; set; }

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

            // Seeddata
            AddSeeddata(modelBuilder);
        }

        private void AddSeeddata(ModelBuilder modelBuilder)
        {
            // Event(s)
            modelBuilder.Entity<Event>().HasData(new Event
            {
                IdEvent = 1,
                Title = "iO Event Connect",
                Description = "Hier zal besproken worden wat er allemaal gemaakt moet worden voor de beste event calender ooit.",
                StartDate = new DateTime(2026, 10, 10, 9, 0, 0),
                EndDate = new DateTime(2026, 10, 10, 17, 0, 0),
                MainColorHex = "#D9D9D9",
                AccentColorHex = "#B0B0B0",
                LogoPath = ""
            });

            // Rooms
            modelBuilder.Entity<Room>().HasData(new Room
            {
                IdRoom = 1,
                IdEvent = 1,
                RoomLabel = "Hoofdzaal",
                Description = "Grote conferentiezaal.",
                Capacity = 100
            });

            modelBuilder.Entity<Room>().HasData(new Room
            {
                IdRoom = 2,
                IdEvent = 1,
                RoomLabel = "Kamer 1",
                Description = "Grote meeting zaal.",
                Capacity = 20
            });

            modelBuilder.Entity<Room>().HasData(new Room
            {
                IdRoom = 3,
                IdEvent = 1,
                RoomLabel = "Kamer 2",
                Description = "Kleine meeting zaal.",
                Capacity = 3
            });

            // Sprekers
            modelBuilder.Entity<Speaker>().HasData(new Speaker
            {
                IdSpeaker = 1,
                FirstName = "Jan",
                LastName = "Smit",
                description = "Expert in C# en Cloud.",
                ImgPath = ""
            });

            modelBuilder.Entity<Speaker>().HasData(new Speaker
            {
                IdSpeaker = 2,
                FirstName = "John",
                LastName = "Doe",
                description = "Expert in Databases en networking.",
                ImgPath = ""
            });

            // Sessions
            modelBuilder.Entity<Session>().HasData(new Session
            {
                IdSession = 1,
                IdEvent = 1,
                IdRoom = 1,
                Title = "Bespreking algemene eisen en wensen.",
                StartTime = new DateTime(2026, 10, 10, 9, 0, 0),
                EndTime = new DateTime(2026, 10, 10, 10, 0, 0),
                Capacity = 0,
                Plenary = true
            });

            // Tags
            modelBuilder.Entity<Tag>().HasData(new Tag
            {
                IdEvent = 1,
                Title = "Plenaire sessie",
                ColorHex = "#D5B82C",
            });

            modelBuilder.Entity<Tag>().HasData(new Tag
            {
                IdEvent = 1,
                Title = "Technology",
                ColorHex = "#2CCFD5",
            });
        }
    }
}
