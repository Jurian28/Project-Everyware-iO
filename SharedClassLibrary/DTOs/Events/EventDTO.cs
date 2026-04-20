using System.ComponentModel.DataAnnotations;

namespace SharedClassLibrary.DTOs.Events;
public class EventDTO
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
    public bool IsPublished { get; set; } = false;


    /// <summary>
    /// Date validation for the event, can be extended if needed
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "The start date cannot be in the past.",
                new[] { nameof(StartDate) }
            );
        }
        if (EndDate < DateTime.Today)
        {
            yield return new ValidationResult(
                "The end date cannot be in the past.",
                new[] { nameof(EndDate) }
            );
        }
        else if (EndDate < StartDate)
        {
            yield return new ValidationResult(
                "The end date cannot be before the start date.",
                new[] { nameof(EndDate) }
            );
        }
    }
}