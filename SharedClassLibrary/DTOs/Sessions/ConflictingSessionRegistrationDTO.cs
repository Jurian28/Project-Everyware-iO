namespace SharedClassLibrary.DTOs.Sessions;

public class ConflictingSessionDTO
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public bool InQueue { get; set; } = false;
}

public class ConflictingSessionRegistrationDTO
{
    public required ConflictingSessionDTO Session { get; set; }
    public required IEnumerable<ConflictingSessionDTO> ConflictingSessions { get; set; }
}

