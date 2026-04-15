public class CUSessionDTO
{
    public SessionDTO? session { get; set; }
    public DateTime? EventStartTime { get; set; }
    public DateTime? EventEndTime { get; set; }

    // has: RoomId, RoomLabel, Capacity
    public List<SessionRoomDTO> AvailableRooms { get; set; } = new List<SessionRoomDTO>();
    // has: EventId, Title
    public List<SessionTagDTO> AvailableTags { get; set; } = new List<SessionTagDTO>();
    // has: SpeakerId, Name
    public List<SessionSpeakerDTO> AvailableSpeakers { get; set; } = new List<SessionSpeakerDTO>();
}