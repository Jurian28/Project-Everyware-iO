using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Update DTO for the Event model.
/// </summary>
namespace SharedClassLibrary.DTOs.Events;

/// <summary>
/// Represents the input DTO used for updating an Event.
/// </summary>
public class EventUpdateDTO : IEventFileDTO
{
    [Required]
    public int IdEvent { get; set; }
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
    public bool IsPublished { get; set; }
    public IFormFile? LogoFile { get; set; }
    public string? LogoPath { get; set; }
}
