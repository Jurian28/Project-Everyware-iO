namespace SharedClassLibrary.DTOs.Sessions
{
    public class SessionListDto
    {
        public int EventId { get; set; }
        public List<SessionDTO> Sessions { get; set; }
    }
}
