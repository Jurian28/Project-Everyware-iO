namespace WebApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
        public required bool RememberMe { get; set; }
    }
}
