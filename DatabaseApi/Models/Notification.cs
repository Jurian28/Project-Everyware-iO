using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

public class Notification
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required User Receiver { get; set; }

    [Required]
    public required DateTime SentAt { get; set; }

    [Required]
    [MinLength(1)]
    public required string Title { get; set; }

    [Required]
    [MinLength(1)]
    public required string Content { get; set; }
}
