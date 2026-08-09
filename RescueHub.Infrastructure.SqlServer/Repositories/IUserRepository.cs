using RescueHub.Domain.Entities;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailOrPhoneAsync(string email, string phoneNumber);
        Task AddAsync(User user);
    }
}