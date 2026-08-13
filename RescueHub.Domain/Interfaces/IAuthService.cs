using RescueHub.Domain.Entities;
using RescueHub.Domain.Enums;

namespace RescueHub.Domain.Interfaces
{
    public interface IAuthService
    {
        Task<(string?, User?)> LoginAsync(
            string email,
            string password,
            CancellationToken cancellationToken);
    }
}