using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

public class EventInvite
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required Event Event { get; set; }

    [Required]
    public required DateTime Expires { get; set; }
}