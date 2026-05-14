using DatabaseApi.Models;

namespace DatabaseApi.DTOs.Speakers;

public class SpeakerMapper
{
    public static Speaker ToEntity(SpeakerInsertDTO dto) => new Speaker
    {
        FirstName = dto.FirstName,
        MiddleName = dto.MiddleName,
        LastName = dto.LastName,
        Description = dto.Description,
        IdEvent = dto.IdEvent
    };

    public static void UpdateEntity(Speaker speaker, SpeakerUpdateDTO dto)
    {
        speaker.FirstName = dto.FirstName;
        speaker.MiddleName = dto.MiddleName;
        speaker.LastName = dto.LastName;
        speaker.Description = dto.Description;
    }

    public static SpeakerResponseDTO ToResponseDTO(Speaker speaker) => new SpeakerResponseDTO
    {
        IdSpeaker = speaker.IdSpeaker,
        FirstName = speaker.FirstName,
        MiddleName = speaker.MiddleName,
        LastName = speaker.LastName,
        Description = speaker.Description,
        ImgPath = speaker.ImgPath,
        IdEvent = speaker.IdEvent
    };
}
