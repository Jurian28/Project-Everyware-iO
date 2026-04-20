using Microsoft.AspNetCore.Http;

namespace SharedClassLibrary.DTOs.Events;

/// <summary>
/// Represents the interface for file data transfer used for creating or updating an Event.
/// </summary>
public interface IEventFileDTO
{
    public IFormFile? LogoFile { get; set; }
}
