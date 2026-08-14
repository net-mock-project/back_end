using System.ComponentModel.DataAnnotations;

namespace RescueHub.API.Models.Authentication
{
    public class ResendOtpRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        public string Email { get; set; } = string.Empty;
    }
}