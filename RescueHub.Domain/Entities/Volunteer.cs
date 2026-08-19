using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Volunteer : BaseEntity
    {
        public Guid VolunteerId => Id;
        public int ExperienceYears { get; private set; }
        public VolunteerApprovalStatus ApprovalStatus { get; private set; }
        public string? CVUrl { get; private set; }
        public Guid? ApprovedBy { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

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
            IEnumerable<VolunteerSkill>? skills = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            ExperienceYears = experienceYears;
            ApprovalStatus = approvalStatus;
            CVUrl = cvUrl;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;

            if (skills != null)
            {
                _skills.AddRange(skills);
            }
        }
    }
}