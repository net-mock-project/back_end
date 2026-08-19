using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Volunteers
{
    public interface IVolunteerRepository
    {
        Task<Volunteer?> GetByIdAsync(
            Guid volunteerId,
            CancellationToken cancellationToken);

        Task AddAsync(
            Volunteer volunteer,
            CancellationToken cancellationToken);

        Task UpdateAsync(
            Volunteer volunteer,
            CancellationToken cancellationToken);

        Task<PagedResult<Volunteer>> GetPendingPagedAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);
    }
}