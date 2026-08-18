using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Auth;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Guid?> GetRoleIdAsync(string name, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name);

            if (role == null)
                return null;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return role.Id;
        }

        public async Task<string?> GetRoleNameAsync(Guid roleId, CancellationToken cancellationToken)
        {
            var role = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == roleId);

            if (role == null)
                return null;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return role.Name;
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Users
                .AsNoTracking()
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToDomain(dataModel);
        }

        public async Task<User?> GetByPhoneAsync(string phone, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u =>
                    u.Phone == phone &&
                    u.DeletedAt == null);

            if (user == null)
                return null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return MapToDomain(user);
        }

        public async Task<bool> AddAsync(User user, CancellationToken cancellationToken)
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

            // Nếu User có GeoLocation, map sang kiểu dữ liệu của NetTopologySuite hoặc tương đương trong SqlServer model của bạn
            if (user.Location != null)
            {
                // Tùy thuộc vào cách bạn định nghĩa GeoLocation trong UserDataModel
                // dataModel.Location = ... 
            }

            await _dbContext.Users.AddAsync(dataModel);
            var affectedRows = await _dbContext.SaveChangesAsync();

            return affectedRows > 0;
        }

        // Helper chuyển đổi từ Data Model sang Domain Entity (tương tự như UserRepository của bạn)
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