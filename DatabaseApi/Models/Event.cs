using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Event
    {
        [Key]
        public int idEvent { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime endDate { get; set; }
        public string description { get; set; }
        public string mainColorHex { get; set; }
        public string accentColorHex { get; set; }

        // Navigation properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
