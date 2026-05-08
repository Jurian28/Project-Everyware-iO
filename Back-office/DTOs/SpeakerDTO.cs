using Microsoft.AspNetCore.Http;

namespace Back_office.DTOs;

public class SpeakerDTO
{
    public int? IdSpeaker { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImgPath { get; set; }
    public int IdEvent { get; set; }
    public IFormFile? Image { get; set; }
}
