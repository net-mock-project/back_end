using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Domain.Common.Querying;

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
        public async Task<User?> GetProfileByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == userId &&
                    u.DeletedAt == null)
                .Select(u => new
                {
                    User = u,
                    RoleName = u.Role.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                return null;
            }

            return MapToDomain(
                result.User,
                result.RoleName);
        }

        // Lấy danh sách User có phân trang
        public async Task<PagedResult<User>> GetPagedAsync(
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

            var totalCount = await query.CountAsync(
                cancellationToken);

            var data = await query
                .OrderByDescending(u => u.CreatedAt)
                .ThenBy(u => u.Id)
                .Skip(
                    (criteria.PageNumber - 1)
                    * criteria.PageSize)
                .Take(criteria.PageSize)
                .Select(u => new
                {
                    User = u,
                    RoleName = u.Role.Name
                })
                .ToListAsync(cancellationToken);

            var items = data
                .Select(x =>
                    MapToDomain(
                        x.User,
                        x.RoleName)!)
                .ToList();

            return new PagedResult<User>(
                items,
                totalCount);
        }

        // Lấy chi tiết User theo Id
        public async Task<User?> GetDetailByIdAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var result = await _dbContext.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id == userId &&
                    u.DeletedAt == null)
                .Select(u => new
                {
                    User = u,

                    RoleName = u.Role.Name,

                    ReliefRequestCount =
                        u.ReliefRequests.Count(
                            r => r.DeletedAt == null),

                    DonationCount =
                        u.Donations.Count(
                            d => d.DeletedAt == null),

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

            if (result == null)
            {
                return null;
            }

            return MapToDomain(
                result.User,
                result.RoleName,
                result.ReliefRequestCount,
                result.DonationCount,
                result.TaskCompletedCount);
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

        // Cập nhật Role của User khi được phê duyệt hoặc thay đổi quyền
        public async Task<bool> UpdateRoleAsync(
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

            existing.RoleId = user.RoleId;
            existing.UpdatedAt = user.UpdatedAt;

            return true;
        }

        // Chuyển Data Model sang Domain Entity
        private User? MapToDomain(
            UserDataModel? dataModel,
            string? roleName = null,
            int reliefRequestCount = 0,
            int donationCount = 0,
            int taskCompletedCount = 0)
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
                dataModel.DeletedAt,
                roleName,
                reliefRequestCount,
                donationCount,
                taskCompletedCount);
        }
    }
}