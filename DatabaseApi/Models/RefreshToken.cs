using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public required string Token { get; set; }

    public required string UserId { get; set; }

    public required DateTime Expires { get; set; }
}
