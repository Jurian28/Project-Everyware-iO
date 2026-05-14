using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Speaker
    {
        [Key]
        public int IdSpeaker { get; set; }
        [Required]
        public int IdEvent { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? Description { get; set; }
        public string? ImgPath { get; set; }
        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
