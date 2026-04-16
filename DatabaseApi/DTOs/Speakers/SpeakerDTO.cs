using DatabaseApi.Models;
using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.DTOs.Speakers
{
    /// <summary>
    /// SpeakerData send in gets and as a response to post/put requests
    /// </summary>
    public class SpeakerResponseDTO
    {
        public int? IdSpeaker { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Description { get; set; }
        public string? ImgPath { get; set; }
        public int IdEvent { get; set; }
    }
    /// <summary>
    /// Represents the input data for creating a Speaker.
    /// </summary>
    public class SpeakerInsertDTO
    {
        [Required(ErrorMessage = "firstName is required.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "lastName is required.")]
        public string LastName { get; set; }

        public string? Description { get; set; }

        public string? ImgPath { get; set; }

        [Required(ErrorMessage = "IdEvent is required.")]
        public int IdEvent { get; set; }

    }
    /// <summary>
    /// Represents the input data for updating a Speaker.
    /// </summary>
    public class SpeakerUpdateDTO
    {
        [Required(ErrorMessage = "firstName is required.")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required(ErrorMessage = "lastName is required.")]
        public string LastName { get; set; }

        public string? Description { get; set; }

        public string? ImgPath { get; set; }
    }
}
