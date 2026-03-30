namespace DatabaseApi.Models.Dtos;

/// <summary>
/// Represents the input data transfer object used for authentication.
/// </summary>
public class AuthInputDto
{
    /// <summary>
    /// Gets or sets the email address used for authentication.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the password used for authentication.
    /// </summary>
    public required string Password { get; set; }
}
