using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Event
    {
        [Key]
        public int IdEvent { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string MainColorHex { get; set; }
        public string AccentColorHex { get; set; }
        public string LogoPath { get; set; }

        // Navigation properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<EventInvite> Invites { get; set; } = new List<EventInvite>();
    }
}
