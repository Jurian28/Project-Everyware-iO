namespace SharedClassLibrary.DTOs.Sessions;

public class SessionSpotsDTO
{
    public int SessionId { get; set; }
    public required string SessionTitle { get; set; }
    public int FilledSpots { get; set; }
    public int TotalSpots { get; set; }
    public int SpotsInWaitingList { get; set; }
    public bool IsPlenarySession { get; set; }
    public List<AttendeeDTO> Attendees { get; set; }
}
