using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Roles;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Interfaces.Volunteers;

namespace RescueHub.Domain.Services
{
    public class VolunteerService : IVolunteerService
    {
        private readonly IVolunteerRepository _volunteerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public VolunteerService(
            IVolunteerRepository volunteerRepository,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _volunteerRepository = volunteerRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
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

        public async Task<Volunteer?> GetProfileByIdForCoordinatorAsync(
            Guid volunteerId,
            Guid coordinatorId,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            if (coordinator == null || string.IsNullOrWhiteSpace(coordinator.Province))
                return null;

            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            if (volunteer == null || volunteer.DeletedAt != null)
                return null;

            // Chỉ trả về nếu cùng khu vực quản lý
            if (!string.Equals(volunteer.Province, coordinator.Province, StringComparison.OrdinalIgnoreCase))
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

            // 1. Chặn nếu hồ sơ đang chờ duyệt hoặc đã được duyệt chính thức
            if (existingVolunteer != null && existingVolunteer.DeletedAt == null &&
               (existingVolunteer.ApprovalStatus == VolunteerApprovalStatus.Pending ||
                existingVolunteer.ApprovalStatus == VolunteerApprovalStatus.Approved))
            {
                return null;
            }

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(volunteerId, s.SkillId, s.Level)
            ).ToList();

            // 2. Khôi phục nếu đã xóa mềm HOẶC nộp lại đơn mới sau khi bị Rejected
            if (existingVolunteer != null && (existingVolunteer.DeletedAt != null || existingVolunteer.ApprovalStatus == VolunteerApprovalStatus.Rejected))
            {
                var reactivatedVolunteer = new Volunteer(
                    volunteerId,
                    experienceYears,
                    VolunteerApprovalStatus.Pending, // Reset về chờ duyệt
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

            // 3. Tạo mới hoàn toàn nếu chưa từng có bản ghi
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
            if (volunteer == null || volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending || volunteer.DeletedAt != null)
                return false;

            return await _volunteerRepository.DeleteAsync(volunteerId, cancellationToken);
        }

        public async Task<PagedResult<Volunteer>> GetPendingProfilesAsync(
            Guid coordinatorId,
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            if (coordinator == null || string.IsNullOrWhiteSpace(coordinator.Province))
                return new PagedResult<Volunteer>(new List<Volunteer>(), 0);

            return await _volunteerRepository.GetPendingPagedAsync(
                coordinator.Province,
                criteria,
                cancellationToken);
        }

        public async Task<PagedResult<Volunteer>> GetApprovedProfilesAsync(
            Guid coordinatorId,
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            if (coordinator == null || string.IsNullOrWhiteSpace(coordinator.Province))
                return new PagedResult<Volunteer>(new List<Volunteer>(), 0);

            return await _volunteerRepository.GetApprovedPagedAsync(
                coordinator.Province,
                criteria,
                cancellationToken);
        }

        public async Task<Volunteer?> ApproveProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken)
        {
            var approver = await _userRepository.GetByIdAsync(approverId, cancellationToken);
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
            var user = await _userRepository.GetByIdAsync(volunteerId, cancellationToken);

            if (approver == null || volunteer == null || user == null ||
                volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending ||
                volunteer.DeletedAt != null)
            {
                return null;
            }

            if (!string.Equals(approver.Province, user.Province, StringComparison.OrdinalIgnoreCase))
                return null;

            var volunteerRole = await _roleRepository.GetByNameAsync("Volunteer", cancellationToken);
            if (volunteerRole == null)
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

            user.ChangeRole(volunteerRole.RoleId);
            await _userRepository.UpdateRoleAsync(user, cancellationToken);

            return updatedVolunteer;
        }

        public async Task<Volunteer?> RejectProfileAsync(
            Guid volunteerId,
            Guid approverId,
            CancellationToken cancellationToken)
        {
            var approver = await _userRepository.GetByIdAsync(approverId, cancellationToken);
            var volunteer = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);

            if (approver == null || volunteer == null ||
                volunteer.ApprovalStatus != VolunteerApprovalStatus.Pending ||
                volunteer.DeletedAt != null)
            {
                return null;
            }

            if (!string.Equals(approver.Province, volunteer.Province, StringComparison.OrdinalIgnoreCase))
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

        public async Task<Volunteer?> CreateByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetUserId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            var targetUser = await _userRepository.GetByIdAsync(targetUserId, cancellationToken);

            if (coordinator == null || targetUser == null)
                return null;

            // Kiểm tra phân quyền theo Tỉnh/Thành
            if (string.IsNullOrWhiteSpace(coordinator.Province) ||
                string.IsNullOrWhiteSpace(targetUser.Province) ||
                !string.Equals(coordinator.Province.Trim(), targetUser.Province.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Lấy Role Volunteer từ Database
            var volunteerRole = await _roleRepository.GetByNameAsync("Volunteer", cancellationToken);
            if (volunteerRole == null)
                return null;

            var existingVolunteer = await _volunteerRepository.GetByIdAsync(targetUserId, cancellationToken);

            // Chỉ chặn nếu người dùng ĐÃ LÀ Tình nguyện viên chính thức đang hoạt động
            if (existingVolunteer != null &&
                existingVolunteer.DeletedAt == null &&
                existingVolunteer.ApprovalStatus == VolunteerApprovalStatus.Approved)
            {
                return null;
            }

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(targetUserId, s.SkillId, s.Level)
            ).ToList();

            var volunteer = new Volunteer(
                targetUserId,
                experienceYears,
                VolunteerApprovalStatus.Approved, // Coordinator chủ động tạo -> Approved luôn
                cvUrl,
                coordinatorId,
                DateTime.UtcNow,
                existingVolunteer?.CreatedAt ?? DateTime.UtcNow,
                DateTime.UtcNow,
                null, // Reset xóa mềm nếu có
                volunteerSkills);

            if (existingVolunteer != null)
            {
                // Đã có bản ghi (từng bị Rejected, Pending, hoặc Soft-deleted) -> Cập nhật lại
                await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
            }
            else
            {
                // Chưa từng có bản ghi -> Thêm mới
                await _volunteerRepository.AddAsync(volunteer, cancellationToken);
            }

            targetUser.ChangeRole(volunteerRole.RoleId);
            await _userRepository.UpdateRoleAsync(targetUser, cancellationToken);

            return volunteer;
        }

        public async Task<Volunteer?> UpdateByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetVolunteerId,
            int experienceYears,
            string? cvUrl,
            IEnumerable<(Guid SkillId, int Level)> skills,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            var volunteer = await _volunteerRepository.GetByIdAsync(targetVolunteerId, cancellationToken);

            if (coordinator == null || volunteer == null || volunteer.DeletedAt != null)
                return null;

            if (!string.Equals(coordinator.Province, volunteer.Province, StringComparison.OrdinalIgnoreCase))
                return null;

            var volunteerSkills = skills.Select(s =>
                new VolunteerSkill(targetVolunteerId, s.SkillId, s.Level)
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

        public async Task<bool> DeleteByCoordinatorAsync(
            Guid coordinatorId,
            Guid targetVolunteerId,
            CancellationToken cancellationToken)
        {
            var coordinator = await _userRepository.GetByIdAsync(coordinatorId, cancellationToken);
            var volunteer = await _volunteerRepository.GetByIdAsync(targetVolunteerId, cancellationToken);
            var user = await _userRepository.GetByIdAsync(targetVolunteerId, cancellationToken);

            if (coordinator == null || volunteer == null || user == null || volunteer.DeletedAt != null)
                return false;

            if (!string.Equals(coordinator.Province, volunteer.Province, StringComparison.OrdinalIgnoreCase))
                return false;

            var requesterRole = await _roleRepository.GetByNameAsync("Requester", cancellationToken);
            if (requesterRole == null)
                return false;

            var deleted = await _volunteerRepository.DeleteAsync(targetVolunteerId, cancellationToken);
            if (deleted)
            {
                user.ChangeRole(requesterRole.RoleId);
                await _userRepository.UpdateRoleAsync(user, cancellationToken);
            }

            return deleted;
        }
    }
}