using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IUserRepository
    {
        // Lấy User theo UserId
        Task<User?> GetByIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        // Kiểm tra Email đã được User khác sử dụng hay chưa
        Task<bool> EmailExistsAsync(
            string email,
            int excludeUserId,
            CancellationToken cancellationToken = default);

        // Lưu các thay đổi xuống Database
        Task SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}