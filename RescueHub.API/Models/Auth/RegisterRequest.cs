namespace RescueHub.API.Models.Auth
{
    public class RegisterRequest
    {
        public required string Email { get; set; }

        public required string OtpCode { get; set; }
    }
}