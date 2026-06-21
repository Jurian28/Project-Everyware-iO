using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class Poll
    {
        [Key]
        public int IdPoll { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsClosed { get; set; } = false;
        [Required]
        public int IdSession { get; set; }

        public Session Session { get; set; }
        public ICollection<PollAnswer> Answers { get; set; } = new List<PollAnswer>();
    }
}
