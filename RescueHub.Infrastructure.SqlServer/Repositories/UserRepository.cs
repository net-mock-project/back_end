using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.ReadModels.Users;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    // Repository thao tác dữ liệu User bằng EF Core
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return dataModel == null
                ? null
                : MapToDomain(dataModel);
        }

        // Lấy thông tin Profile kèm RoleName
        public async Task<UserProfileItem?> GetProfileByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == userId &&
                    u.DeletedAt == null)
                .Select(u => new UserProfileItem
                {
                    Id = u.Id,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Province = u.Province,
                    ProfileUrl = u.ProfileUrl
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        // Lấy danh sách User có phân trang
        public async Task<PagedResult<UserListItem>> GetPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Users
                .AsNoTracking()
                .Where(u => u.DeletedAt == null);

            // Tìm theo tên, email hoặc số điện thoại
            if (!string.IsNullOrWhiteSpace(criteria.Search))
            {
                var search = criteria.Search.Trim();

                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Phone.Contains(search));
            }

            // Tổng số User trước khi phân trang
            var totalCount = await query.CountAsync(
                cancellationToken);

            // Lấy dữ liệu của trang hiện tại
            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .ThenBy(u => u.Id)
                .Skip(
                    (criteria.PageNumber - 1)
                    * criteria.PageSize)
                .Take(criteria.PageSize)
                .Select(u => new UserListItem
                {
                    Id = u.Id,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Province = u.Province,
                    Status = u.Status,
                    IsVerified = u.IsVerified,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResult<UserListItem>(
                items,
                totalCount);
        }

        // Lấy chi tiết User theo Id
        public async Task<UserDetailItem?> GetDetailByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == userId &&
                    u.DeletedAt == null)
                .Select(u => new UserDetailItem
                {
                    Id = u.Id,
                    RoleId = u.RoleId,
                    RoleName = u.Role.Name,

                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    ProfileUrl = u.ProfileUrl,

                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Province = u.Province,

                    Status = u.Status,
                    IsVerified = u.IsVerified,

                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,

                    // Tổng số yêu cầu cứu trợ do User tạo
                    ReliefRequestCount = u.ReliefRequests.Count(
                        r => r.DeletedAt == null),

                    // Tổng số Donation của User
                    DonationCount = u.Donations.Count(
                        d => d.DeletedAt == null),

                    // Tổng số Task Volunteer đã hoàn thành
                    TaskCompletedCount =
                        u.Volunteer == null
                            ? 0
                            : u.Volunteer.TaskAssignments.Count(
                                a =>
                                    a.DeletedAt == null &&
                                    a.Status ==
                                        TaskAssignmentStatus.Completed)
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            // Khóa row User trong transaction hiện tại
            var existing = await _dbContext.Users
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM [Users] WITH (UPDLOCK, ROWLOCK)
                    WHERE Id = {user.Id}")
                .FirstOrDefaultAsync(cancellationToken);


            if (existing == null)
            {
                return false;
            }

            existing.FullName = user.FullName;
            existing.Phone = user.Phone;
            existing.DateOfBirth = user.DateOfBirth;
            existing.Gender = user.Gender;
            existing.UpdatedAt = user.UpdatedAt;

            return true;
        }


        // Cập nhật URL avatar của User
        public async Task<bool> UpdateAvatarAsync(
            User user,
            CancellationToken cancellationToken)
        {
            // Khóa row User trong transaction hiện tại
            var existing = await _dbContext.Users
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM [Users] WITH (UPDLOCK, ROWLOCK)
                    WHERE Id = {user.Id}")
                .FirstOrDefaultAsync(cancellationToken);

            if (existing == null)
            {
                return false;
            }

            // Cập nhật ảnh đại diện
            existing.ProfileUrl = user.ProfileUrl;
            existing.UpdatedAt = user.UpdatedAt;

            return true;
        }

        // Kiểm tra Email đã tồn tại
        public async Task<bool> EmailExistsAsync(
            string email,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    u => u.Email.ToLower() == email.ToLower()
                         && u.DeletedAt == null,
                    cancellationToken);
        }


        // Kiểm tra số điện thoại đã tồn tại
        public async Task<bool> PhoneExistsAsync(
            string phone,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    u => u.Phone == phone
                         && u.DeletedAt == null,
                    cancellationToken);
        }


        // Kiểm tra Role tồn tại
        public async Task<bool> RoleExistsAsync(
            Guid roleId,
            CancellationToken cancellationToken)
        {
            return await _dbContext.Roles
                .AsNoTracking()
                .AnyAsync(
                    r => r.Id == roleId,
                    cancellationToken);
        }


        // Thêm User mới
        public async Task AddAsync(
            User user,
            CancellationToken cancellationToken)
        {
            var dataModel = new UserDataModel
            {
                Id = user.Id,
                RoleId = user.RoleId,
                Province = user.Province,
                ProfileUrl = user.ProfileUrl,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PasswordHash = user.PasswordHash,
                Status = user.Status,
                IsVerified = user.IsVerified,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt
            };

            await _dbContext.Users.AddAsync(
                dataModel,
                cancellationToken);
        }

        // Cập nhật trạng thái User
        public async Task<bool> UpdateStatusAsync(
            User user,
            CancellationToken cancellationToken)
        {
            // Khóa row User trong transaction hiện tại
            var existing = await _dbContext.Users
                .FromSqlInterpolated($@"
            SELECT *
            FROM [Users] WITH (UPDLOCK, ROWLOCK)
            WHERE Id = {user.Id}")
                .FirstOrDefaultAsync(cancellationToken);

            if (existing == null)
            {
                return false;
            }

            existing.Status = user.Status;
            existing.UpdatedAt = user.UpdatedAt;

            return true;
        }

        // Chuyển Data Model sang Domain Entity
        private User? MapToDomain(UserDataModel? dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            GeoLocation? location = null;

            if (dataModel.Location != null)
            {
                location = new GeoLocation(
                    dataModel.Location.Y,
                    dataModel.Location.X);
            }

            return new User(
                dataModel.Id,
                dataModel.RoleId,
                location,
                dataModel.Province,
                dataModel.ProfileUrl,
                dataModel.FullName,
                dataModel.Email,
                dataModel.Phone,
                dataModel.DateOfBirth,
                dataModel.Gender,
                dataModel.PasswordHash,
                dataModel.Status,
                dataModel.IsVerified,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt);
        }
    }
}