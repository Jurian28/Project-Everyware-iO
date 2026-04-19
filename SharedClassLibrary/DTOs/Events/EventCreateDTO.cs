using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Events;

/// <summary>
/// Represents the input DTO used for creating an Event.
/// </summary>
public class EventCreateDto : IEventFileDTO
{
    [Required]
    public string Title { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public string Location { get; set; }
    public string? Description { get; set; }
    [Required]
    public string MainColorHex { get; set; }
    [Required]
    public string AccentColorHex { get; set; }

    public IFormFile? LogoFile { get; set; }
}
