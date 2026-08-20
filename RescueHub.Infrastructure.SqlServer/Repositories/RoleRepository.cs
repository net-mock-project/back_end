using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Roles;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public RoleRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Lấy Role theo tên vai trò
        public async Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.Name.ToLower() == name.ToLower(),
                    cancellationToken);

            return dataModel == null
                ? null
                : MapToDomain(dataModel);
        }

        // Lấy Role theo RoleId
        public async Task<Role?> GetByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    r => r.Id == roleId,
                    cancellationToken);

            return dataModel == null
                ? null
                : MapToDomain(dataModel);
        }

        // Lấy toàn bộ danh sách Role trong hệ thống
        public async Task<IReadOnlyList<Role>> GetAllAsync(
            CancellationToken cancellationToken)
        {
            var dataModels = await _dbContext.Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return dataModels
                .Select(MapToDomain)
                .Where(r => r != null)
                .Select(r => r!)
                .ToList();
        }

        // Chuyển Data Model sang Domain Entity
        private Role? MapToDomain(RoleDataModel? dataModel)
        {
            if (dataModel == null)
            {
                return null;
            }

            return new Role(
                dataModel.Id,
                dataModel.Name,
                dataModel.Description);
        }
    }
}