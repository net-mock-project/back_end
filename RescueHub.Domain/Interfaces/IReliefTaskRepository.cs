using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IReliefTaskRepository
    {
        Task<ReliefTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReliefTask>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ReliefTask>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
        Task AddAsync(ReliefTask task, CancellationToken cancellationToken = default);
        Task UpdateAsync(ReliefTask task, CancellationToken cancellationToken = default);
        Task DeleteAsync(ReliefTask task, CancellationToken cancellationToken = default);
    }
}
