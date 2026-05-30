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
        public DbSet<SessionReview> SessionReviews { get; set; }
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

            modelBuilder.Entity<Tag>()
            .HasIndex(t => new { t.IdEvent, t.Title })
            .IsUnique();

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
        }
    }
}
