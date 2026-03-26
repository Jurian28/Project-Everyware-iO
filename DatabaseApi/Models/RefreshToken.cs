using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string UserId { get; set; }

    [Required]
    public required DateTime Expires { get; set; }
}
