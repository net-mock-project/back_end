using RescueHub.Domain.Common.Constants;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Domain.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IUserRepository _userRepository;

        public VolunteerService(
            IVolunteerRepository volunteerRepository,
            IUserRepository userRepository)
        {
            _volunteerRepository = volunteerRepository;
            _userRepository = userRepository;
        }

        public async Task<Volunteer?> GetProfileAsync(
            Guid volunteerId,
            CancellationToken cancellationToken)
        {
            return await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        }

        public async Task<Volunteer?> CreateProfileAsync(
            Guid volunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken)
        {
            var existingVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (existingVolunteer != null)
                return null;

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(volunteerId, s.SkillId, s.Level)
            ).ToList();

            var volunteer = new Volunteer(
                volunteerId,
                experienceYears,
                VolunteerApprovalStatus.Pending,
                cvUrl,
                null,
                null,
                DateTime.UtcNow,
                null,
                null,
                volunteerSkills);

            await _volunteerRepository.AddAsync(volunteer, cancellationToken);
            return volunteer;
        }

        public Task<PagedResult<Volunteer>> GetPendingProfilesAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            return _volunteerRepository.GetPendingPagedAsync(
                criteria,
                cancellationToken);
        }

        public async Task<Volunteer?> ApproveProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer == null || volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending)
                return null;

            var user = await _userRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (user == null)
                return null;

            var updatedVolunteer = new Volunteer(
                volunteer.VolunteerId,
                volunteer.ExperienceYears,
                VolunteerApprovalStatus.Approved,
                volunteer.CVUrl,
                approverId,
                DateTime.UtcNow,
                volunteer.CreatedAt,
                DateTime.UtcNow,
                null,
                volunteer.Skills);

            await _volunteerRepository.UpdateAsync(updatedVolunteer, cancellationToken);

            user.ChangeRole(RoleConstants.VolunteerId);
            await _userRepository.UpdateRoleAsync(user, cancellationToken);

            return updatedVolunteer;
        }

        public async Task<Volunteer?> RejectProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer == null || volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending)
                return null;

            var updatedVolunteer = new Volunteer(
                volunteer.VolunteerId,
                volunteer.ExperienceYears,
                VolunteerApprovalStatus.Rejected,
                volunteer.CVUrl,
                approverId,
                DateTime.UtcNow,
                volunteer.CreatedAt,
                DateTime.UtcNow,
                null,
                volunteer.Skills);

            await _volunteerRepository.UpdateAsync(updatedVolunteer, cancellationToken);
            return updatedVolunteer;
        }
    }
}