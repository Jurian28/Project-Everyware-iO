using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Tag
    {
        [Key]
        public int IdTag { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ColorHex { get; set; }
        [Required]
        public int IdEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
