using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class VolunteerEngagement : BaseEntity
    {
        public Guid VolunteerId { get; private set; }
        public Guid RequestId { get; private set; }
        public VolunteerEngagementStatus Status { get; private set; }

        public Volunteer Volunteer { get; private set; } = null!;
        public ReliefRequest Request { get; private set; } = null!;

        private VolunteerEngagement() { }

        public VolunteerEngagement(
            Guid id,
            Guid volunteerId,
            Guid requestId,
            VolunteerEngagementStatus status,
            DateTime createdAt,
            DateTime? updatedAt = null,
            DateTime? deletedAt = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            VolunteerId = volunteerId;
            RequestId = requestId;
            Status = status;
        }

        public void ChangeStatus(VolunteerEngagementStatus newStatus)
        {
            Status = newStatus;
            MarkUpdated();
        }

        public void Activate()
        {
            Status = VolunteerEngagementStatus.Active;
            MarkUpdated();
        }

        public void Cancel()
        {
            Status = VolunteerEngagementStatus.Cancelled;
            MarkUpdated();
        }
    }
}
