using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class ReliefTask : BaseEntity
    {
        public Guid RequestId { get; private set; }
        public string Title { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public int RequiredVolunteers { get; private set; }
        public int Priority { get; private set; }
        public GeoLocation? Location { get; private set; }
        public ReliefTaskStatus Status { get; private set; }
        public List<Guid> TaskSkills { get; private set; } = new();

        public ReliefRequest Request { get; private set; } = null!;
        public IReadOnlyCollection<TaskAssignment> Assignments => _assignments.AsReadOnly();
        private readonly List<TaskAssignment> _assignments = new();

        private ReliefTask() { }

        public ReliefTask(
            Guid id,
            Guid requestId,
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            ReliefTaskStatus status,
            List<Guid> taskSkills,
            DateTime createdAt,
            DateTime? updatedAt = null,
            DateTime? deletedAt = null)
            : base(id, createdAt, updatedAt, deletedAt)
            {
                RequestId = requestId;
                Title = title;
                Description = description;
                RequiredVolunteers = requiredVolunteers;
                Priority = priority;
                Location = location;
                Status = status;
                TaskSkills = taskSkills ?? new List<Guid>();
            }

        public void UpdateDetails(
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            List<Guid> taskSkills)
        {
            Title = title;
            Description = description;
            RequiredVolunteers = requiredVolunteers;
            Priority = priority;
            Location = location;
            TaskSkills = taskSkills ?? new List<Guid>();
            MarkUpdated();
        }

        public void ChangeStatus(ReliefTaskStatus newStatus)
        {
            Status = newStatus;
            MarkUpdated();
        }
    }
}
