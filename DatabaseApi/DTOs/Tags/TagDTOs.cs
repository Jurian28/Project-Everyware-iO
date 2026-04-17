using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.DTOs.Tags
{
    /// <summary>
    /// TagData sent in GETs and as a response to POST/PUT requests.
    /// </summary>
    public class TagResponseDTO
    {
        public int IdTag { get; set; }
        public int IdEvent { get; set; }
        public string Title { get; set; }
        public string? ColorHex { get; set; }
    }

    /// <summary>
    /// Represents the input data for creating a Tag.
    /// </summary>
    public class TagInsertDTO
    {
        [Required(ErrorMessage = "IdEvent is required.")]
        public int IdEvent { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        public required string Title { get; set; }

        public string? ColorHex { get; set; }
    }

    /// <summary>
    /// Represents the input data for updating a Tag.
    /// </summary>
    public class TagUpdateDTO
    {
        public required string Title { get; set; }
        public required string ColorHex { get; set; }
    }
}
