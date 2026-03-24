using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Speaker
    {
        [Key]
        public int IdSpeaker { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? description { get; set; }
        public string? ImgPath { get; set; }

        // Navigation properties
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
