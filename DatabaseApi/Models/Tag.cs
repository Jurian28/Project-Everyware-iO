using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    [PrimaryKey(nameof(title), nameof(idEvent))]
    public class Tag
    {
        [Required]
        public string title { get; set; }
        public string colorHex { get; set; }
        [Required]
        public int idEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
