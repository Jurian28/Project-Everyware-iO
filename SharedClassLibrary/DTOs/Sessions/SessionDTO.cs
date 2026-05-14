using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Tags;

public class SessionDTO
{
    public int SessionId { get; set; }
    public string Title { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool Plenary { get; set; }
    public int? PlacesLeft { get; set; }
    public bool IsEnrolled { get; set; } = false;

    public int IdRoom { get; set; }
    public RoomResponseDTO? Room { get; set; }
    public List<TagResponseDTO> Tags { get; set; }
    public int? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
}