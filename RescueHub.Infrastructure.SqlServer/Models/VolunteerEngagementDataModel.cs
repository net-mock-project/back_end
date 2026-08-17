using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class VolunteerEngagementDataModel
    {
        public Guid Id { get; set; }

        public Guid VolunteerId { get; set; }

        public Guid RequestId { get; set; }

        public string? PerformanceAssessment { get; set; }

        public VolunteerEngagementStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public VolunteerDataModel Volunteer { get; set; } = null!;

        public ReliefRequestDataModel Request { get; set; } = null!;
    }
}
