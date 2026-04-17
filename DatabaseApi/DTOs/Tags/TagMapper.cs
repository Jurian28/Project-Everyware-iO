using DatabaseApi.Models;

namespace DatabaseApi.DTOs.Tags
{
    public class TagMapper
    {
        public static Tag ToEntity(TagInsertDTO dto)
        {
            return new Tag
            {
                IdEvent = dto.IdEvent,
                Title = dto.Title,
                ColorHex = dto.ColorHex ?? "#000000"
            };
        }

        public static void UpdateEntity(Tag tag, TagUpdateDTO dto)
        {
            tag.Title = dto.Title;
            tag.ColorHex = dto.ColorHex;
        }

        public static TagResponseDTO ToResponseDTO(Tag tag)
        {
            return new TagResponseDTO
            {
                IdEvent = tag.IdEvent,
                Title = tag.Title,
                ColorHex = tag.ColorHex
            };
        }
    }
}