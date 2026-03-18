using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Room
    {
        [Key]
        public int idRoom { get; set; }
        public string roomLabel { get; set; }
        public int capacity { get; set; }
        public string description { get; set; }
        public int idEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
