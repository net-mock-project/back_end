using NetTopologySuite.Geometries;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class ReliefRequestDataModel
    {
        public Guid Id { get; set; }

        public Guid RequesterId { get; set; }

        public Guid? CoordinatorId { get; set; }

        public Point Location { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? ReliefImageUrl { get; set; }

        public string? RequestedResource { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int UrgencyLevel { get; set; }

        public int EstimatedAffectedPeople { get; set; }

        public decimal? EstimatedAffectedRadiusKm { get; set; }

        public ReliefRequestStatus Status { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public UserDataModel Requester { get; set; } = null!;

        public UserDataModel? Coordinator { get; set; }

        public ICollection<ReliefTaskDataModel> Tasks { get; set; }
            = new List<ReliefTaskDataModel>();

        public ICollection<VolunteerEngagementDataModel> VolunteerEngagements { get; set; }
            = new List<VolunteerEngagementDataModel>();
    }
}
