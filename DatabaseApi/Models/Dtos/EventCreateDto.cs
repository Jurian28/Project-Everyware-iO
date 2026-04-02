using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models.Dtos
{
    /// <summary>
    /// Represents the input data transfer object used for creating an Event.
    /// </summary>
    public class EventCreateDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string Location { get; set; }
        public string Description { get; set; }
        [Required]
        public string MainColorHex { get; set; }
        [Required]
        public string AccentColorHex { get; set; }

        // Include the file directly in the DTO
        public IFormFile? LogoFile { get; set; }
    }
}
