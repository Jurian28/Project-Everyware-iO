using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class User_has_Session
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int idSession { get; set; }
        [Required]
        public bool inWaitingList { get; set; }
        [Required]
        public DateTime joinedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Session Session { get; set; }
    }
}
