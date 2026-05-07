namespace SharedClassLibrary.DTOs.Rooms;

/// <summary>
/// RoomData send in gets and as a response to post/put requests
/// </summary>
public class RoomResponseDTO
{
    public int IdRoom { get; set; }
    public required string RoomLabel { get; set; }
    public int Capacity { get; set; }
    public string? Description { get; set; }
    public int IdEvent { get; set; }
}