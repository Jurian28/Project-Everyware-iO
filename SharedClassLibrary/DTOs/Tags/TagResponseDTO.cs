namespace SharedClassLibrary.DTOs.Tags;

/// <summary>
/// TagData sent in GETs and as a response to POST/PUT requests.
/// </summary>
public class TagResponseDTO
{
    public int IdTag { get; set; }
    public int IdEvent { get; set; }
    public string Title { get; set; }
    public string? ColorHex { get; set; }
}

