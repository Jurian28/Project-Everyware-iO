using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Session
    {
        [Key]
        public int IdSession { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public bool Plenary { get; set; }
        [Required]
        public int IdEvent { get; set; }
        [Required]
        public int IdRoom { get; set; }
        public bool StartingNotificationSent { get; set; } = false;
        public DateTime? StartNotificationSendTime { get; set; } = null;

        // Navigation properties
        public Event Event { get; set; }
        public Room Room { get; set; }
        public ICollection<Speaker> Speakers { get; set; } = new List<Speaker>();
        public ICollection<User_has_Session> RegisteredUsers { get; set; } = new List<User_has_Session>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
