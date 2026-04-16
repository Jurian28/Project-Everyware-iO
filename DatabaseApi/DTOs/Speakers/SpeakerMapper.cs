using DatabaseApi.Models;

namespace DatabaseApi.DTOs.Rooms
{
    public class RoomMapper
    {
        public static Speaker ToEntity(RoomInsertDTO dto) => new Speaker
        {
            FirstName = dto.RoomLabel,
            MiddleName = dto.Capacity,
            LastName = dto.Capacity,
            Description = dto.Description,
            ImgPath = dto.ImgPath,
            IdEvent = dto.IdEvent
        };

        public static void UpdateEntity(Speaker speaker, SpeakerUpdateDTO dto)
        {
            speaker.FirstName = dto.RoomLabel,
            speaker.MiddleName = dto.Capacity,
            speaker.LastName = dto.Capacity,
            speaker.Description = dto.Description,
            speaker.ImgPath = dto.ImgPath,
        }

        public static SpeakerResponseDTO ToResponseDTO(Speaker speaker) => new SpeakerResponseDTO
        {
            FirstName = speaker.RoomLabel,
            MiddleName = speaker.Capacity,
            LastName = speaker.Capacity,
            Description = speaker.Description,
            ImgPath = speaker.ImgPath,
            IdEvent = speaker.IdEvent
        };
    }
}
