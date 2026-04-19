namespace SharedClassLibrary.DTOs.Events;
public class EventListDto
{
    public int TotalPages { get; set; }
    public List<EventDTO> Events { get; set; } = new List<EventDTO>();
}