using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Volunteer : BaseEntity
    {
        public Guid VolunteerId { get; private set; }

        public int ExperienceYears { get; private set; }

        public VolunteerApprovalStatus ApprovalStatus { get; private set; }

        public string? CVUrl { get; private set; }

        public Guid? ApprovedBy { get; private set; }

        public DateTime? ApprovedAt { get; private set; }

        private Volunteer() { }

        // Dùng khi dựng lại Volunteer đã tồn tại từ database
        public Volunteer(
            Guid volunteerId,
            int experienceYears,
            VolunteerApprovalStatus approvalStatus,
            string? cvUrl,
            Guid? approvedBy,
            DateTime? approvedAt,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt)
            : base(volunteerId, createdAt, updatedAt, deletedAt)
        {
            VolunteerId = volunteerId;
            ExperienceYears = experienceYears;
            ApprovalStatus = approvalStatus;
            CVUrl = cvUrl;
            ApprovedBy = approvedBy;
            ApprovedAt = approvedAt;
        }
    }
}