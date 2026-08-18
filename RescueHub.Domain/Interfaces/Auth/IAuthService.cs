using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Auth
{
    public interface IAuthService
    {
        Task ValidateNewUserRegistrationAsync(string email, CancellationToken cancellationToken);

        Task<bool> RegisterAsync(
            string address,
            string fullName,
            string email,
            String phoneNumber,
            DateTime dateOfBirth,
            Gender gender,
            string passwordHash,
            CancellationToken cancellationToken);

        Task<(User?, string?)> LoginAsync(
            string email,
            CancellationToken cancellationToken);
    }
}