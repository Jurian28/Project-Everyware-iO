using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Session
    {
        [Key]
        public int idSession { get; set; }
        public string title { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public int planSize { get; set; }
        public int capacity { get; set; }
        public int idEvent { get; set; }
        public int idRoom { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Room Room { get; set; }
        public ICollection<Speaker> Speakers { get; set; } = new List<Speaker>();
        public ICollection<User_has_Session> Users { get; set; } = new List<User_has_Session>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
