using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext để Repository thao tác với Database
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy User theo UserId
        public async Task<User?> GetByIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.UserId == userId
                         && x.DeleteAt == null,
                    cancellationToken);
        }

        // Kiểm tra Email có đang được User khác sử dụng không
        public async Task<bool> EmailExistsAsync(
            string email,
            int excludeUserId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(
                    x => x.Email == email
                         && x.UserId != excludeUserId
                         && x.DeleteAt == null,
                    cancellationToken);
        }

        // Lưu các thay đổi xuống Database
        public async Task SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}