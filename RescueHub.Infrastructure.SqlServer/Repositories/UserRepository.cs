using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;


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

        public async Task<bool> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            // Khóa row User trong transaction hiện tại
            var existing = await _dbContext.Users
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM [User] WITH (UPDLOCK, ROWLOCK)
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
                    FROM [User] WITH (UPDLOCK, ROWLOCK)
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
                dataModel.DeleteAt);
        }
    }
}