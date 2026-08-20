using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces
{
    public interface ITaskAssignmentRepository
    {
        Task<TaskAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TaskAssignment>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken = default);
        Task AddAsync(TaskAssignment assignment, CancellationToken cancellationToken = default);
        Task UpdateAsync(TaskAssignment assignment, CancellationToken cancellationToken = default);
        Task DeleteAsync(TaskAssignment assignment, CancellationToken cancellationToken = default);
    }
}
