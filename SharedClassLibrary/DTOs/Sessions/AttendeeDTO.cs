namespace SharedClassLibrary.DTOs.Sessions;

public class AttendeeDTO
{
    public string IdUser { get; set; }
    public bool InWaitingList { get; set; }
    public DateTime JoinedDate { get; set; }
    public string UserName { get; set; }
    public bool IsAttending { get; set; }
}
