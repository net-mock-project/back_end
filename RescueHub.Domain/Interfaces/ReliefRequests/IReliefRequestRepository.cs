using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.ReliefRequests
{
    public interface IReliefRequestRepository
    {
        Task<ReliefRequest> AddAsync(ReliefRequest request, CancellationToken cancellationToken);
        Task<ReliefRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<ReliefRequest>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<ReliefRequest>> GetByRequesterIdAsync(Guid requesterId, CancellationToken cancellationToken);
        Task UpdateAsync(ReliefRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(ReliefRequest request, CancellationToken cancellationToken);
    }
}
