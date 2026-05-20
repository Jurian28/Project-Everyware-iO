namespace SharedClassLibrary.DTOs.Sessions;

public class AttendeeDTO
{
    // TODO update when new db field added
    public string IdUser { get; set; }
    public bool InWaitingList { get; set; }
    public DateTime JoinedDate { get; set; }
    public string UserName { get; set; }
}
