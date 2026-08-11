using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserService
    {

        // Cập nhật thông tin Profile
        Task<User?> UpdateProfileAsync(
            Guid userId,
            string? fullName,
            string? phone,
            DateOnly? dateOfBirth,
            Gender? gender);
    }
}