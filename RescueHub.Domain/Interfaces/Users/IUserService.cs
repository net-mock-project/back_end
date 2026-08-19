using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.ReadModels.Users;

namespace RescueHub.Domain.Interfaces.Users
{
    public interface IUserService
    {
        // Lấy thông tin Profile
        Task<UserProfileItem?> GetProfileAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Lấy danh sách User có phân trang
        Task<PagedResult<UserListItem>> GetUsersAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        // Lấy chi tiết User cho Admin
        Task<UserDetailItem?> GetUserDetailAsync(
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

        // Admin tạo User mới
        Task<User> CreateUserAsync(
            Guid roleId,
            string? province,
            string fullName,
            string email,
            string phone,
            DateOnly? dateOfBirth,
            Gender? gender,
            string passwordHash,
            CancellationToken cancellationToken);

        // Admin khóa tài khoản User
        Task<User?> LockUserAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Admin mở khóa tài khoản User
        Task<User?> UnlockUserAsync(
            Guid userId,
            CancellationToken cancellationToken);
    }
}