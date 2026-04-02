using System.ComponentModel.DataAnnotations;

namespace Back_office.Models
{
    public class Event : IValidatableObject
    {
        public int IdEvent { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [MaxLength(250)]
        public string Location { get; set; }
        [MaxLength(5000)]
        public string? Description { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(7)]
        public string MainColorHex { get; set; }
        [Required]
        [MinLength(4)]
        [MaxLength(7)]
        public string AccentColorHex { get; set; }
        public string? LogoPath { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<User> Users { get; set; } = new List<User>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate < DateTime.Today)
            {
                yield return new ValidationResult(
                    "The start date cannot be in the past.",
                    new[] { nameof(StartDate) }
                );
            } 
            if (EndDate < StartDate)
            {
                yield return new ValidationResult(
                    "The end date cannot be in the past.",
                    new[] { nameof(EndDate) }
                );
            }
        }
    }
}
