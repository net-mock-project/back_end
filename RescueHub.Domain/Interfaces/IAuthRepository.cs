using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);

        Task AddOtpAsync(OtpVerification otp);
        Task<OtpVerification?> GetLatestOtpByEmailAsync(string email);
        Task RemoveOtpAsync(OtpVerification otp);
        Task RemoveOldOtpAsync(string email);

        Task SaveChangesAsync();
    }
}