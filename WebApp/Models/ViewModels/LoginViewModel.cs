namespace WebApp.Models.ViewModels;

/// <summary>
/// Represents the view model used for user login.
/// </summary>
public class LoginViewModel
{
    /// <summary>
    /// Gets or sets the email address associated with the user account.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the password for the user account.
    /// </summary>
    public required string Password { get; set; }
}
