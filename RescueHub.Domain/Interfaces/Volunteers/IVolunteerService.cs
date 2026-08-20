using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Volunteers
{
    public interface IVolunteerService
    {
        Task<Volunteer?> GetProfileAsync(
            Guid volunteerId,
            CancellationToken cancellationToken);

        Task<Volunteer?> GetProfileByIdForCoordinatorAsync(
            Guid volunteerId,
            Guid coordinatorId,
            CancellationToken cancellationToken);

        Task<Volunteer?> CreateProfileAsync(
            Guid volunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken);

        Task<Volunteer?> UpdateProfileAsync(
            Guid volunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken);

        Task<bool> CancelProfileAsync(
            Guid volunteerId,
            CancellationToken cancellationToken);

        Task<PagedResult<Volunteer>> GetPendingProfilesAsync(
            Guid coordinatorId,
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        Task<PagedResult<Volunteer>> GetApprovedProfilesAsync(
            Guid coordinatorId,
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        Task<Volunteer?> ApproveProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken);

        Task<Volunteer?> RejectProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken);

        // CRUD dành riêng cho Coordinator
        Task<Volunteer?> CreateByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetUserId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken);

        Task<Volunteer?> UpdateByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetVolunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken);

        Task<bool> DeleteByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetVolunteerId,
            CancellationToken cancellationToken);
    }
}