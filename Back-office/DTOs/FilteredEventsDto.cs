using Back_office.Models;

namespace Back_office.DTOs
{
    /// <summary>
    /// DTO for all events and pagination information.
    /// </summary>
    public class FilteredEventsDto
    {
        public required List<Event> Events { get; set; }
        public int TotalPages { get; set; }
    }
}
