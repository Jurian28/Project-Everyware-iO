using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class User_has_Session
    {
        [Required]
        public string IdUser { get; set; }
        [Required]
        public int IdSession { get; set; }
        [Required]
        public bool InWaitingList { get; set; }
        [Required]
        public DateTime JoinedDate { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Session Session { get; set; }
    }
}
