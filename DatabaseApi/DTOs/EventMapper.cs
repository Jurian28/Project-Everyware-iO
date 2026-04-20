using DatabaseApi.Models;
using SharedClassLibrary.DTOs.Events;

namespace DatabaseApi.DTOs
{
    public class EventMapper
    {
        public static EventDTO ToResponseDTO(Event evt) => new EventDTO {
            IdEvent = evt.IdEvent,
            Title = evt.Title,
            StartDate = evt.StartDate,
            EndDate = evt.EndDate,
            Location = evt.Location,
            Description = evt.Description,
            MainColorHex = evt.MainColorHex,
            AccentColorHex = evt.AccentColorHex,
            LogoPath = evt.LogoPath,
            IsPublished = evt.IsPublished
        };
    }
}