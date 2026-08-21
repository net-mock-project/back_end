using RescueHub.Domain.Entities;
using RescueHub.Domain.Common.Querying;

namespace RescueHub.Domain.Interfaces.Users
{
    public interface IUserRepository
    {
        // Lấy User theo Id
        Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

        // Lấy thông tin Profile kèm Role
        Task<User?> GetProfileByIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Lấy danh sách User có phân trang
        Task<PagedResult<User>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        // Lấy chi tiết User theo Id
        Task<User?> GetDetailByIdAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Cập nhật User
        Task<bool> UpdateAsync(User user, CancellationToken cancellationToken);

        // Cập nhật URL avatar của User
        Task<bool> UpdateAvatarAsync(
            User user,
            CancellationToken cancellationToken);

        // Kiểm tra Email đã tồn tại
        Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken);

        // Kiểm tra số điện thoại đã tồn tại
        Task<bool> PhoneExistsAsync(
            string phone,
            CancellationToken cancellationToken);

        // Kiểm tra Role tồn tại
        Task<bool> RoleExistsAsync(
            Guid roleId,
            CancellationToken cancellationToken);

        // Thêm User mới
        Task AddAsync(
            User user,
            CancellationToken cancellationToken);

        // Cập nhật trạng thái User
        Task<bool> UpdateStatusAsync(
            User user,
            CancellationToken cancellationToken);

        // Cập nhật Role của User
        Task<bool> UpdateRoleAsync(
            User user,
            CancellationToken cancellationToken);


        Task<bool> UpdateLocationAsync(
            User user,
            CancellationToken cancellationToken);

        Task<List<User>> GetUsersWithinRangeAsync(
            double latitude,
            double longitude,
            double radius,
            CancellationToken cancellationToken);
    }
}