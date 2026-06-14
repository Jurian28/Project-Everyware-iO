using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models
{
    public class PollAnswer
    {
        [Key]
        public int IdPollAnswer { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public int IdPoll { get; set; }

        public Poll Poll { get; set; }
    }
}
