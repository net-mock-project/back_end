using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Lấy User theo Id
        Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

        // Cập nhật User
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);

        // Cập nhật URL avatar của User
        Task<bool> UpdateAvatarAsync(
            User user,
            CancellationToken cancellationToken);
    }
}