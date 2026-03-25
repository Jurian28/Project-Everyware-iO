using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Room
    {
        [Key]
        public int IdRoom { get; set; }
        [Required]
        public required string RoomLabel { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        [Required]
        public int IdEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
