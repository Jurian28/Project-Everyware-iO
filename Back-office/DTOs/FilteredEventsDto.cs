using Back_office.Models;

namespace Back_office.DTOs
{
    public class FilteredEventsDto
    {
        public required List<Event> Events { get; set; }
        public int TotalPages { get; set; }
    }
}
