using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    [PrimaryKey(nameof(Title), nameof(IdEvent))]
    public class Tag
    {
        [Required]
        public string Title { get; set; }
        public string ColorHex { get; set; }
        [Required]
        public int IdEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
