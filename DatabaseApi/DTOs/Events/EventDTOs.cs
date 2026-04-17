using System.ComponentModel.DataAnnotations;

/// <summary>
/// All DTOs for the Event model.
/// </summary>
namespace DatabaseApi.DTOs.Events
{
    /// <summary>
    /// Represents the interface for file data transfer used for creating or updating an Event.
    /// </summary>
    public interface EventFileDto { 
        public IFormFile? LogoFile { get; set; } 
    }

    /// <summary>
    /// Represents the input DTO used for creating an Event.
    /// </summary>
    public class EventCreateDto : EventFileDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
        [Required]
        public string MainColorHex { get; set; }
        [Required]
        public string AccentColorHex { get; set; }

        public IFormFile? LogoFile { get; set; }
    }

    /// <summary>
    /// Represents the input DTO used for updating an Event.
    /// </summary>
    public class EventUpdateDto : EventFileDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
        [Required]
        public string MainColorHex { get; set; }
        [Required]
        public string AccentColorHex { get; set; }
        public bool IsPublished { get; set; }


        public IFormFile? LogoFile { get; set; }
        public string? LogoPath { get; set; }
    }

    /// <summary>
    /// Represents the input DTO used for publishing or unpublishing an Event.
    /// </summary>
    public class PublishEventDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool Publish { get; set; }
    }
}
