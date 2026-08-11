using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserService
    {

        // Cập nhật thông tin Profile
        Task<User?> UpdateProfileAsync(
            Guid userId,
            string? fullName,
            string? phone,
            string? province);
    }
}