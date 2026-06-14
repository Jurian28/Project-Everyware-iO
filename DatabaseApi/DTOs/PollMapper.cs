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

        public static PollDTO ToResponseDTO(Poll poll)
        {
            return new PollDTO
            {
                IdPoll = poll.IdPoll,
                Title = poll.Title,
                Description = poll.Description,
                IsClosed = poll.IsClosed,
                IdSession = poll.IdSession,
                Answers = poll.Answers.Select(a => new PollAnswerDTO
                {
                    IdPollAnswer = a.IdPollAnswer,
                    Text = a.Text
                }).ToList()
            };
        }
    }
}
