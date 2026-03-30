namespace WebApp.Models.Dtos;

/// <summary>
/// Represents the data transfer object containing authentication tokens.
/// </summary>
public class AuthOutputDto
{
    /// <summary>
    /// Gets or sets the JWT access token used for authenticating subsequent API requests.
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Gets or sets the refresh token used to acquire a new access token when the current one expires.
    /// </summary>
    public required string RefreshToken { get; set; }
}
