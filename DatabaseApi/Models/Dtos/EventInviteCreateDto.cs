namespace DatabaseApi.Models.Dtos;

public class EventInviteCreateDto
{
    public required int EventId { get; set; }

    public required DateTime Expires { get; set; }
}
