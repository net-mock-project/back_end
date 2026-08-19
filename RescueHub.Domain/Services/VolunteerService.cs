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
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            if (volunteer == null || volunteer.DeletedAt != null)
                return null;

            return volunteer;
        }

        public async Task<Volunteer?> CreateProfileAsync(
            Guid volunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken)
        {
            var existingVolunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            // 1. Đã có hồ sơ và đang hoạt động -> Không cho tạo đè
            if (existingVolunteer != null && existingVolunteer.DeletedAt == null)
                return null;

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(volunteerId, s.SkillId, s.Level)
            ).ToList();

            // 2. Đã từng có hồ sơ nhưng đã bị xóa mềm -> Khôi phục và cập nhật lại thông tin mới
            if (existingVolunteer != null && existingVolunteer.DeletedAt != null)
            {
                var reactivatedVolunteer = new Volunteer(
                    volunteerId,
                    experienceYears,
                    VolunteerApprovalStatus.Pending,
                    cvUrl,
                    approvedBy: null,
                    approvedAt: null,
                    createdAt: existingVolunteer.CreatedAt,
                    updatedAt: DateTime.UtcNow,
                    deletedAt: null, // Reset xóa mềm
                    skills: volunteerSkills,
                    fullName: existingVolunteer.FullName,
                    email: existingVolunteer.Email,
                    phone: existingVolunteer.Phone,
                    profileUrl: existingVolunteer.ProfileUrl,
                    province: existingVolunteer.Province);

                await _volunteerRepository.UpdateAsync(reactivatedVolunteer, cancellationToken);
                return reactivatedVolunteer;
            }

            // 3. Chưa từng tạo hồ sơ -> Tạo mới hoàn toàn (INSERT)
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

        public async Task<Volunteer?> UpdateProfileAsync(
            Guid volunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            // Không cho phép cập nhật hồ sơ đã bị xóa mềm
            if (volunteer == null || volunteer.DeletedAt != null)
                return null;

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(volunteerId, s.SkillId, s.Level)
            ).ToList();

            var updatedVolunteer = new Volunteer(
                volunteer.VolunteerId,
                experienceYears,
                volunteer.ApprovalStatus,
                cvUrl,
                volunteer.ApprovedBy,
                volunteer.ApprovedAt,
                volunteer.CreatedAt,
                DateTime.UtcNow,
                volunteer.DeletedAt,
                volunteerSkills,
                volunteer.FullName,
                volunteer.Email,
                volunteer.Phone,
                volunteer.ProfileUrl,
                volunteer.Province);

            await _volunteerRepository.UpdateAsync(updatedVolunteer, cancellationToken);
            return updatedVolunteer;
        }

        public async Task<bool> CancelProfileAsync(
            Guid volunteerId,
            CancellationToken cancellationToken)
        {
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            // Chỉ cho phép hủy khi hồ sơ đang chờ duyệt
            if (volunteer == null || volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending)
            {
                return false;
            }

            return await _volunteerRepository.DeleteAsync(volunteerId, cancellationToken);
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

        public Task<PagedResult<Volunteer>> GetApprovedProfilesAsync(
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            return _volunteerRepository.GetApprovedPagedAsync(
                criteria,
                cancellationToken);
        }
    }
}