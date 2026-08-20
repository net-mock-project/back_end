using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class ReliefRequest : BaseEntity
    {
        public Guid RequesterId { get; private set; }
        public Guid? CoordinatorId { get; private set; }
        public GeoLocation Location { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public string? ReliefImageUrl { get; private set; }
        public string? RequestedResource { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int UrgencyLevel { get; private set; }
        public int EstimatedAffectedPeople { get; private set; }
        public decimal? EstimatedAffectedRadiusKm { get; private set; }
        public ReliefRequestStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public User Requester { get; private set; } = null!;
        public User? Coordinator { get; private set; }

        private ReliefRequest() { }

        public ReliefRequest(
            Guid id,
            Guid requesterId,
            GeoLocation location,
            string title,
            string description,
            string? reliefImageUrl,
            string? requestedResource,
            int urgencyLevel,
            int estimatedAffectedPeople,
            decimal? estimatedAffectedRadiusKm,
            ReliefRequestStatus status,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            RequesterId = requesterId;
            Location = location;
            Title = title;
            Description = description;
            ReliefImageUrl = reliefImageUrl;
            RequestedResource = requestedResource;
            UrgencyLevel = urgencyLevel;
            EstimatedAffectedPeople = estimatedAffectedPeople;
            EstimatedAffectedRadiusKm = estimatedAffectedRadiusKm;
            Status = status;
        }

        public void UpdateDetails(
            string title,
            string description,
            string? reliefImageUrl,
            string? requestedResource,
            int urgencyLevel,
            int estimatedAffectedPeople,
            decimal? estimatedAffectedRadiusKm,
            GeoLocation location)
        {
            Title = title;
            Description = description;
            ReliefImageUrl = reliefImageUrl;
            RequestedResource = requestedResource;
            UrgencyLevel = urgencyLevel;
            EstimatedAffectedPeople = estimatedAffectedPeople;
            EstimatedAffectedRadiusKm = estimatedAffectedRadiusKm;
            Location = location;
            MarkUpdated();
        }

        public void Approve(Guid coordinatorId)
        {
            if (Status != ReliefRequestStatus.Pending)
                throw new InvalidOperationException($"Cannot approve request in status {Status}");
            
            Status = ReliefRequestStatus.Approved;
            CoordinatorId = coordinatorId;
            MarkUpdated();
        }

        public void Reject(Guid coordinatorId)
        {
            if (Status != ReliefRequestStatus.Pending)
                throw new InvalidOperationException($"Cannot reject request in status {Status}");

            Status = ReliefRequestStatus.Rejected;
            CoordinatorId = coordinatorId;
            MarkUpdated();
        }

        public void Complete(Guid coordinatorId)
        {
            if (Status != ReliefRequestStatus.Approved && Status != ReliefRequestStatus.InProgress)
                throw new InvalidOperationException($"Cannot complete request in status {Status}");

            Status = ReliefRequestStatus.Completed;
            CoordinatorId = coordinatorId;
            CompletedAt = DateTime.UtcNow;
            MarkUpdated();
        }

        public void Cancel()
        {
            if (Status != ReliefRequestStatus.Pending)
                throw new InvalidOperationException($"Cannot cancel request in status {Status}");

            Status = ReliefRequestStatus.Cancelled;
            MarkUpdated();
        }
    }
}
