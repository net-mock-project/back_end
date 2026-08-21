using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Domain.Services
{
    public class ReliefTaskService : IReliefTaskService
    {
        private readonly IReliefTaskRepository _taskRepository;
        private readonly ITaskAssignmentRepository _assignmentRepository;
        private readonly IVolunteerEngagementRepository _engagementRepository;

        public ReliefTaskService(
            IReliefTaskRepository taskRepository,
            ITaskAssignmentRepository assignmentRepository,
            IVolunteerEngagementRepository engagementRepository)
        {
            _taskRepository = taskRepository;
            _assignmentRepository = assignmentRepository;
            _engagementRepository = engagementRepository;
        }

        public async Task<ReliefTask> CreateTaskAsync(
            Guid requestId,
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            List<Guid> taskSkills,
            CancellationToken cancellationToken)
        {
            var task = new ReliefTask(
                Guid.NewGuid(),
                requestId,
                title,
                description,
                requiredVolunteers,
                priority,
                location,
                ReliefTaskStatus.Pending,
                taskSkills,
                DateTime.UtcNow
            );
            await _taskRepository.AddAsync(task, cancellationToken);
            return task;
        }

        public async Task<ReliefTask?> UpdateTaskAsync(
            Guid taskId,
            string title,
            string description,
            int requiredVolunteers,
            int priority,
            GeoLocation? location,
            List<Guid> taskSkills,
            CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null) return null;

            task.UpdateDetails(title, description, requiredVolunteers, priority, location, taskSkills);
            await _taskRepository.UpdateAsync(task, cancellationToken);
            return task;
        }

        public async Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null) return false;

            await _taskRepository.DeleteAsync(task, cancellationToken);
            return true;
        }

        public async Task<ReliefTask?> CompleteTaskAsync(Guid taskId, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(taskId, cancellationToken);
            if (task == null) return null;

            task.ChangeStatus(ReliefTaskStatus.Completed);
            await _taskRepository.UpdateAsync(task, cancellationToken);
            return task;
        }

        public async Task<TaskAssignment> AssignVolunteerAsync(
            Guid taskId,
            Guid volunteerId,
            Guid assignedBy,
            TaskAssignmentSource source,
            CancellationToken cancellationToken)
        {
            var status = source == TaskAssignmentSource.Coordinator 
                ? TaskAssignmentStatus.Accepted 
                : TaskAssignmentStatus.Pending;

            var assignment = new TaskAssignment(
                Guid.NewGuid(),
                taskId,
                volunteerId,
                assignedBy,
                source,
                status,
                DateTime.UtcNow
            );

            await _assignmentRepository.AddAsync(assignment, cancellationToken);
            return assignment;
        }

        public async Task<TaskAssignment> InviteVolunteerAsync(
            Guid taskId,
            Guid volunteerId,
            Guid assignedBy,
            CancellationToken cancellationToken)
        {
            var assignment = new TaskAssignment(
                Guid.NewGuid(),
                taskId,
                volunteerId,
                assignedBy,
                TaskAssignmentSource.Coordinator,
                TaskAssignmentStatus.Pending,
                DateTime.UtcNow
            );

            await _assignmentRepository.AddAsync(assignment, cancellationToken);
            return assignment;
        }

        public async Task<VolunteerEngagement> RegisterAvailabilityAsync(
            Guid volunteerId,
            Guid requestId,
            CancellationToken cancellationToken)
        {
            var existing = await _engagementRepository.GetByVolunteerAndRequestAsync(volunteerId, requestId, cancellationToken);
            if (existing != null)
            {
                if (existing.Status != VolunteerEngagementStatus.Active)
                {
                    existing.Activate();
                    await _engagementRepository.UpdateAsync(existing, cancellationToken);
                }
                return existing;
            }

            var engagement = new VolunteerEngagement(
                Guid.NewGuid(),
                volunteerId,
                requestId,
                VolunteerEngagementStatus.Active,
                DateTime.UtcNow
            );
            await _engagementRepository.AddAsync(engagement, cancellationToken);
            return engagement;
        }

        public async Task<bool> CancelAvailabilityAsync(
            Guid volunteerId,
            Guid requestId,
            CancellationToken cancellationToken)
        {
            var existing = await _engagementRepository.GetByVolunteerAndRequestAsync(volunteerId, requestId, cancellationToken);
            if (existing == null) return false;

            existing.Cancel();
            await _engagementRepository.UpdateAsync(existing, cancellationToken);
            return true;
        }
    }
}
