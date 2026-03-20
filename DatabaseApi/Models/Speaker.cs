using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Speaker
    {
        [Key]
        public int idSpeaker { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        public string description { get; set; }
        public string imgPath { get; set; }

        // Navigation properties
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
