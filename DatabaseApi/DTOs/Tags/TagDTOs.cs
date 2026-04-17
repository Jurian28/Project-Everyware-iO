using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.DTOs.Tags
{

    public class TagResponseDTO
    {
        public int IdTag { get; set; }
        public int IdEvent { get; set; }
        public string Title { get; set; }
        public string? ColorHex { get; set; }
    }


    public class TagInsertDTO
    {
        [Required(ErrorMessage = "IdEvent is required.")]
        public int IdEvent { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public required string Title { get; set; }
        public string? ColorHex { get; set; }
    }


    public class TagUpdateDTO
    {
        public required string Title { get; set; }
        public required string ColorHex { get; set; }
    }
}
