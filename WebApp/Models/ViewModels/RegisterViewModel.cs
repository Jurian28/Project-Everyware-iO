namespace WebApp.Models.ViewModels;

/// <summary>
/// Represents the data required for a user to register an account.
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's chosen password.
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the confirmation password, which must match the chosen password.
    /// </summary>
    public required string ConfirmPassword { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user's session should be remembered.
    /// </summary>
    public required bool RememberMe { get; set; }
}
