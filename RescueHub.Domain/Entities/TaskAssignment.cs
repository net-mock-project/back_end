using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class TaskAssignment : BaseEntity
    {
        public Guid TaskId { get; private set; }
        public Guid VolunteerId { get; private set; }
        public Guid AssignedBy { get; private set; }
        public TaskAssignmentSource Source { get; private set; }
        public TaskAssignmentStatus Status { get; private set; }

        public ReliefTask Task { get; private set; } = null!;
        public Volunteer Volunteer { get; private set; } = null!;

        private TaskAssignment() { }

        public TaskAssignment(
            Guid id,
            Guid taskId,
            Guid volunteerId,
            Guid assignedBy,
            TaskAssignmentSource source,
            TaskAssignmentStatus status,
            DateTime createdAt,
            DateTime? updatedAt = null,
            DateTime? deletedAt = null)
            : base(id, createdAt, updatedAt, deletedAt)
        {
            TaskId = taskId;
            VolunteerId = volunteerId;
            AssignedBy = assignedBy;
            Source = source;
            Status = status;
        }

        public void ChangeStatus(TaskAssignmentStatus newStatus)
        {
            Status = newStatus;
            MarkUpdated();
        }
    }
}
