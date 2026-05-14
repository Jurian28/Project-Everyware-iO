using SharedClassLibrary.DTOs.Rooms;
using SharedClassLibrary.DTOs.Tags;

namespace SharedClassLibrary.DTOs.Sessions
{
    public class CUSessionDTO
    {
        public SessionDTO? session { get; set; }
        public DateTime? EventStartTime { get; set; }
        public DateTime? EventEndTime { get; set; }

        // has: RoomId, RoomLabel, Capacity
        public List<RoomResponseDTO> AvailableRooms { get; set; } = new List<RoomResponseDTO>();
        // has: IdTag, IdEvent, Title, ColorHex
        public List<TagResponseDTO> AvailableTags { get; set; } = new List<TagResponseDTO>();
        // has: SpeakerId, Name
        public List<SessionSpeakerDTO> AvailableSpeakers { get; set; } = new List<SessionSpeakerDTO>();
    }
}
