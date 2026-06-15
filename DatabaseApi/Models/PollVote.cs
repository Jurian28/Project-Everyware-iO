using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class PollVote
    {
        [Key]
        public int IdPollVote { get; set; }

        [Required]
        public string IdUser { get; set; }

        [Required]
        public int IdPoll { get; set; }

        [Required]
        public int IdPollAnswer { get; set; }

        public PollAnswer PollAnswer { get; set; }
    }
}
