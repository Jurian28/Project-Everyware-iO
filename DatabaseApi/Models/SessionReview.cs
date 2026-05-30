using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseApi.Models;

public class SessionReview
{
    [Key]
    public int Id { get; set; }
    [Required]
    public int IdSession { get; set; }

    [Required]
    [Range(1, 5, ErrorMessage = "Stars must be between 1 and 5.")]
    public required int stars { get; set; }
    public string comment { get; set; }

    // navigation property
    [ForeignKey(nameof(IdSession))]
    public Session Session { get; set; }
}
