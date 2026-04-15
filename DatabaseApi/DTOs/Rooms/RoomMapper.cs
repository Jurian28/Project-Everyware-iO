using DatabaseApi.Models;

namespace DatabaseApi.DTOs.Rooms
{
    public class RoomMapper
    {
        public static Room ToEntity(RoomInsertDTO dto) => new Room
        {
            RoomLabel = dto.RoomLabel,
            Capacity = dto.Capacity,
            Description = dto.Description,
            IdEvent = dto.IdEvent
        };

        public static void UpdateEntity(Room room, RoomUpdateDTO dto)
        {
            room.RoomLabel = dto.RoomLabel;
            room.Capacity = dto.Capacity;
            room.Description = dto.Description;
        }

        public static RoomResponseDTO ToResponseDTO(Room room) => new RoomResponseDTO
        {
            IdRoom = room.IdRoom,
            RoomLabel = room.RoomLabel,
            Capacity = room.Capacity,
            Description = room.Description,
            IdEvent = room.IdEvent
        };
    }
}
