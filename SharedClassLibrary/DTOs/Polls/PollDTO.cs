namespace SharedClassLibrary.DTOs.Polls;

public class PollDTO
{
    public int IdPoll { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public bool IsClosed { get; set; }
    public int IdSession { get; set; }
    public List<PollAnswerDTO> Answers { get; set; } = new();
}
