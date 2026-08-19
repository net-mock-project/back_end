using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Volunteers
{
    public interface IVolunteerService
    {
        // Lấy hồ sơ Volunteer
        Task<Volunteer?> GetProfileAsync(
            Guid volunteerId,
            CancellationToken cancellationToken);

        // Đăng ký hồ sơ Volunteer
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

        Task<PagedResult<Volunteer>> GetPendingProfilesAsync(
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

        Task<PagedResult<Volunteer>> GetApprovedProfilesAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken);
    }
}