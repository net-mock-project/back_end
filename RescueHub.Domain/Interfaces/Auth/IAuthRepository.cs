using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Auth
{
    public interface IAuthRepository
    {
        // Lấy roleId
        Task<Guid?> GetRoleIdAsync(string name, CancellationToken cancellationToken);

        // Kiểm tra xem email đã tồn tại trong hệ thống chưa
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

        // Kiểm tra số điện thoại đã tồn tại
        Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken);

        // Thêm mới một User vào Database khi đăng ký thành công
        Task<bool> AddAsync(User user, CancellationToken cancellationToken);
    }
}