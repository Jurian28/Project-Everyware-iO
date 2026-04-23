using System.ComponentModel.DataAnnotations;

namespace Back_office.Models.ViewModels;

/// <summary>
/// Represents the data required for a user to register an account.
/// </summary>
public class RegisterViewModel
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// </summary>
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public required string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's chosen password.
    /// </summary>
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    /// <summary>
    /// Gets or sets the confirmation password, which must match the chosen password.
    /// </summary>
    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; }
}
