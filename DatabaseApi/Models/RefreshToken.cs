using System.ComponentModel.DataAnnotations;

namespace DatabaseApi.Models;

/// <summary>
/// Represents a refresh token used for re-authenticating a user without requiring credentials.
/// </summary>
public class RefreshToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the refresh token.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the token string.
    /// </summary>
    [Required]
    public required string Token { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user associated with this token.
    /// </summary>
    [Required]
    public required string UserId { get; set; }

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    [Required]
    public required DateTime Expires { get; set; }
}
