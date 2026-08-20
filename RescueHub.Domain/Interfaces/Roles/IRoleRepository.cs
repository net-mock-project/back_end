using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Roles
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(
            string name,
            CancellationToken cancellationToken);

        Task<Role?> GetByIdAsync(
            Guid roleId,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<Role>> GetAllAsync(
            CancellationToken cancellationToken);
    }
}