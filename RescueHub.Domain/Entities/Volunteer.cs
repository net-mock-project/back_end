using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Volunteer : BaseEntity
    {
        public Guid VolunteerId => Id;
        public GeoLocation? Location { get; private set; }
        public int ExperienceYears { get; private set; }
        public VolunteerApprovalStatus ApprovalStatus { get; private set; }
        public string? CVUrl { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        // Thông tin đính kèm từ User
        public string? FullName { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? ProfileUrl { get; private set; }
        public string? Province { get; private set; }

        private readonly List<VolunteerSkill> _skills = new();
        public IReadOnlyCollection<VolunteerSkill> Skills => _skills.AsReadOnly();

        public Volunteer(
            Guid id,
            int experienceYears,
            VolunteerApprovalStatus approvalStatus,
            string? cvUrl,
            Guid? approvedBy,
            DateTime? approvedAt,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt,
            IEnumerable<VolunteerSkill>? skills = null,
            string? fullName = null,
            string? email = null,
            string? phone = null,
            string? profileUrl = null,
            string? province = null,
            Common.Enums.GeoLocation? location = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            ExperienceYears = experienceYears;
            ApprovalStatus = approvalStatus;
            CVUrl = cvUrl;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
            FullName = fullName;
            Email = email;
            Phone = phone;
            ProfileUrl = profileUrl;
            Province = province;
            Location = location;

            if (skills != null)
            {
                _skills.AddRange(skills);
            }
        }
    }
}