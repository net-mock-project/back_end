using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Auth
{
    public class SendOtpRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public bool IsAgreeTerms { get; set; }
    }
}