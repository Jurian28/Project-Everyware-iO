using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Tag
    {
        [Key]
        public string idtag { get; set; }
        public string colorHex { get; set; }
        public int Event_idEvent { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
