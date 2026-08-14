using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IAuthRepository
    {
        // Kiểm tra xem email đã tồn tại trong hệ thống chưa
        Task<User?> GetByEmailAsync(string email);

        // Kiểm tra số điện thoại đã tồn tại
        Task<User?> GetByPhoneAsync(string phone);

        // Thêm mới một User vào Database khi đăng ký thành công
        Task<bool> AddAsync(User user);
    }
}