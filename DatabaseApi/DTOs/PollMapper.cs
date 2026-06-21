using SharedClassLibrary.DTOs.Polls;
using DatabaseApi.Models;

namespace DatabaseApi.DTOs
{
    public class PollMapper
    {
        public static Poll ToEntity(PollCreateDTO dto)
        {
            return new Poll
            {
                Title = dto.Title,
                Description = dto.Description,
                IdSession = dto.IdSession,
                Answers = dto.Answers.Select(text => new PollAnswer
                {
                    Text = text
                }).ToList()
            };
        }

        public static PollDTO ToResponseDTO(Poll poll, string? userId = null)
        {
            return new PollDTO
            {
                IdPoll = poll.IdPoll,
                Title = poll.Title,
                Description = poll.Description,
                IsClosed = poll.IsClosed,
                IdSession = poll.IdSession,
                HasVoted = userId != null && poll.Answers.Any(a => a.Votes.Any(v => v.IdUser == userId)),
                VotedAnswerId = userId != null
                    ? poll.Answers.SelectMany(a => a.Votes).FirstOrDefault(v => v.IdUser == userId)?.IdPollAnswer
                    : null,
                Answers = poll.Answers.Select(a => new PollAnswerDTO
                {
                    IdPollAnswer = a.IdPollAnswer,
                    Text = a.Text,
                    VoteCount = a.Votes.Count
                }).ToList()
            };
        }
    }
}
