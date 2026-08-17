using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class VolunteerDataModel
    {
        public Guid Id { get; set; }

        public int ExperienceYears { get; set; }

        public VolunteerApprovalStatus ApprovalStatus { get; set; }

        public string? CVUrl { get; set; }

        public Guid? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public UserDataModel User { get; set; } = null!;

        public UserDataModel? Approver { get; set; }

        public ICollection<VolunteerSkillDataModel> VolunteerSkills { get; set; }
            = new List<VolunteerSkillDataModel>();

        public ICollection<VolunteerEngagementDataModel> Engagements { get; set; }
            = new List<VolunteerEngagementDataModel>();

        public ICollection<TaskAssignmentDataModel> TaskAssignments { get; set; }
            = new List<TaskAssignmentDataModel>();
    }
}
