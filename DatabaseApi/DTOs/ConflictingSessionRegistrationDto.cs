using DatabaseApi.Models;

namespace DatabaseApi.DTOs;

public class ConflictingSessionDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public bool InQueue { get; set; } = false;
}

public class ConflictingSessionRegistrationDto
{
    public required ConflictingSessionDto Session { get; set; }
    public required IEnumerable<ConflictingSessionDto> ConflictingSessions { get; set; }
}

