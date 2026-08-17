using RescueHub.Domain.Common.Enums;

namespace RescueHub.Application.Contracts.Auth
{
    public record PendingRegistrationDto(
        string FullName,
        DateTime DateOfBirth,
        string Email,
        string PhoneNumber,
        Gender Gender,
        string PasswordHash,
        string Address,
        string OtpCode,
        DateTime ExpiresAt
    );
}
