using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task AddOtpAsync(OtpVerification otp)
        {
            await _context.OtpVerifications.AddAsync(otp);
        }

        public async Task<OtpVerification?> GetLatestOtpByEmailAsync(string email)
        {
            return await _context.OtpVerifications
                .Where(o => o.Email == email)
                .OrderByDescending(o => o.ExpiredAt)
                .FirstOrDefaultAsync();
        }

        public async Task RemoveOtpAsync(OtpVerification otp)
        {
            _context.OtpVerifications.Remove(otp);
            await Task.CompletedTask;
        }

        public async Task RemoveOldOtpAsync(string email)
        {
            // Lấy tất cả các bản ghi OTP cũ của email này trong DB
            var oldOtps = await _context.OtpVerifications
                .Where(o => o.Email == email)
                .ToListAsync();

            if (oldOtps.Any())
            {
                _context.OtpVerifications.RemoveRange(oldOtps);
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}