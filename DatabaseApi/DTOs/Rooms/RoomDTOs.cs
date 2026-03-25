using DatabaseApi.Models;
using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.DTOs.Rooms
{
    public class RoomDTOs
    {
        public int IdRoom { get; set; }
        public string RoomLabel { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public int IdEvent { get; set; }
    }

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

    public class RoomUpdateDTO
    {
        [Required(ErrorMessage = "RoomLabel is required.")]
        public string RoomLabel { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than 0.")]
        public int Capacity { get; set; }
        public string? Description { get; set; }
    }
}
