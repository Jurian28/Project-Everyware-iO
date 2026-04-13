public class SessionDTO
{
    public int SessionId { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Plenary { get; set; }
    public int? Capacity { get; set; }
    public int? IdRoom { get; set; }
    public string? RoomName { get; set; }
    // Has: EventId, Title
    public List<SessionTagDTO> Tags { get; set; }
    public int? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
}