using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class TaskAssignmentDataModel
    {
        public Guid Id { get; set; }

        public Guid TaskId { get; set; }

        public Guid VolunteerId { get; set; }

        public Guid AssignedBy { get; set; }

        public TaskAssignmentSource AssignmentSource { get; set; }

        public TaskAssignmentStatus Status { get; set; }

        public DateTime AssignedAt { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public DateTime? ResponseAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public ReliefTaskDataModel Task { get; set; } = null!;

        public VolunteerDataModel Volunteer { get; set; } = null!;

        public UserDataModel Assigner { get; set; } = null!;
    }
}
