using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
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

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            var dataModel = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            return MapToDomain(dataModel);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            // Lấy bản ghi hiện tại để cập nhật Profile
            var existing = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (existing == null)
            {
                return false;
            }

            existing.FullName = user.FullName;
            existing.Phone = user.Phone;
            existing.DateOfBirth = user.DateOfBirth;
            existing.Gender = user.Gender;
            existing.UpdatedAt = user.UpdatedAt;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
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