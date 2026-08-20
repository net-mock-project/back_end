using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface IReliefTaskService
    {
        Task<ReliefTask> CreateTaskAsync(
            Guid requestId,
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            List<Guid> taskSkills,
            CancellationToken cancellationToken);

        Task<ReliefTask?> UpdateTaskAsync(
            Guid taskId,
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            List<Guid> taskSkills,
            CancellationToken cancellationToken);

        Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken);

        Task<ReliefTask?> CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken);

        Task<TaskAssignment> AssignVolunteerAsync(
            Guid taskId,
            Guid volunteerId,
            Guid assignedBy,
            TaskAssignmentSource source,
            CancellationToken cancellationToken);

        Task<TaskAssignment> InviteVolunteerAsync(
            Guid taskId,
            Guid volunteerId,
            Guid assignedBy,
            CancellationToken cancellationToken);
            
        Task<VolunteerEngagement> RegisterAvailabilityAsync(
            Guid volunteerId,
            Guid requestId,
            CancellationToken cancellationToken);

        Task<bool> CancelAvailabilityAsync(
            Guid volunteerId,
            Guid requestId,
            CancellationToken cancellationToken);
    }
}
