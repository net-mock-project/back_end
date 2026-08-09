using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserService
    {
        // Cập nhật thông tin Profile của User
        Task<User> UpdateProfileAsync(
            int userId,
            string? fullName,
            string? email,
            string? phone,
            string? province,
            CancellationToken cancellationToken = default);
    }
}