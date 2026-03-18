using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Tag
    {
        [Key]
        public string title { get; set; }
        [Required]
        public string colorHex { get; set; }
        [Key]
        [Required]
        public int idEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
