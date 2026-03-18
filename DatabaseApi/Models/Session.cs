using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Session
    {
        [Key]
        public int idSession { get; set; }
        [Required]
        public string title { get; set; }
        [Required]
        public DateTime startTime { get; set; }
        [Required]
        public DateTime endTime { get; set; }
        [Required]
        public bool plenary { get; set; }
        public int capacity { get; set; }
        [Required]
        public int idEvent { get; set; }
        [Required]
        public int idRoom { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Room Room { get; set; }
        public ICollection<Speaker> Speakers { get; set; } = new List<Speaker>();
        public ICollection<User_has_Session> RegisteredUsers { get; set; } = new List<User_has_Session>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
