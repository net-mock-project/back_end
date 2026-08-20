using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IVolunteerEngagementRepository
    {
        Task<VolunteerEngagement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<VolunteerEngagement?> GetByVolunteerAndRequestAsync(Guid volunteerId, Guid requestId, CancellationToken cancellationToken = default);
        Task<IEnumerable<VolunteerEngagement>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
        Task<IEnumerable<VolunteerEngagement>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken = default);
        Task AddAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default);
        Task UpdateAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default);
        Task DeleteAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default);
    }
}
