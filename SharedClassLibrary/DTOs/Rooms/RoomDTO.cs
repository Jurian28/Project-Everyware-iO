namespace Back_office.DTOs;

/// <summary>
/// DTO that is the same as RoomResponseDTO apart from not having an evenId. this DTO is primarily used in the back-office project
/// </summary>
public class RoomDTO
{
    public int? IdRoom { get; set; }
    public string RoomLabel { get; set; }
    public int Capacity { get; set; }
    public string Description { get; set; }
    public int IdEvent { get; set; }
}
