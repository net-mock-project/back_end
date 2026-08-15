using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;

namespace RescueHub.Domain.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<bool> SendOtpAsync(
            string fullName,
            DateTime dateOfBirth,
            string email,
            string phoneNumber,
            Gender gender,
            string password,
            string address);

        Task<bool> ResendOtpAsync(string email);

        Task<bool> RegisterAsync(
            string email,
            string otpCode);
    
        Task<(string?, User?)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);
    }
}