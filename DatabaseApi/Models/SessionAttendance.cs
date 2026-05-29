using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class SessionAttendance
    {
        [Required]
        public int IdSession { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public bool IsAttending { get; set; }
    }
}
