using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Rooms;

/// <summary>
/// Represents the input data for updating a Room.
/// </summary>
public class RoomUpdateDTO
{
    [Required(ErrorMessage = "RoomLabel is required.")]
    public string? RoomLabel { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
    public int Capacity { get; set; }

    public string? Description { get; set; }
}