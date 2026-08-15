using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;

namespace RescueHub.Domain.Interfaces.Users
{
    public interface IUserService
    {
        // Lấy thông tin Profile
        Task<User?> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Cập nhật thông tin Profile
        Task<User?> UpdateProfileAsync(
            Guid userId,
            string? fullName,
            string? phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            CancellationToken cancellationToken);

        // Cập nhật avartar
        Task<User?> UpdateAvatarAsync(
            Guid userId,
            string profileUrl,
            CancellationToken cancellationToken);
    }
}