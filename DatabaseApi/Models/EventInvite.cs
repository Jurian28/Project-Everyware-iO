using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

/// <summary>
/// Represents an invitation that grants access to a specific event.
/// </summary>
public class EventInvite
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(9)]
    public string Token { get; set; } = string.Empty;

    [Required]
    public required Event Event { get; set; }

    [Required]
    public required DateTime Expires { get; set; }
}
