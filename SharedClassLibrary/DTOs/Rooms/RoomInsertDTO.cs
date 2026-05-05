using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Rooms;

/// <summary>
/// Represents the input data for creating a Room.
/// </summary>
public class RoomInsertDTO
{
    [Required(ErrorMessage = "RoomLabel is required.")]
    public string RoomLabel { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    public string? Description { get; set; }

    [Required(ErrorMessage = "IdEvent is required.")]
    public int IdEvent { get; set; }
}