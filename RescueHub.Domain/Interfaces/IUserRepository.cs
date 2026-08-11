using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Lấy User theo Id
        Task<User?> GetByIdAsync(Guid userId);

        // Cập nhật User
        Task<bool> UpdateAsync(User user);
    }
}