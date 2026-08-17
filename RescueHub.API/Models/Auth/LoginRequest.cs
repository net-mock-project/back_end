using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Auth
{
    public class LoginRequest
    {
        [Required(
            ErrorMessage = "Email is required.")]
        [EmailAddress(
            ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; } = string.Empty;

        [Required(
            ErrorMessage = "Password is required.")]
        [MinLength(
            6,
            ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}