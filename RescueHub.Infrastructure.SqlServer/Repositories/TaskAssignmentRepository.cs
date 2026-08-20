using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class TaskAssignmentRepository : ITaskAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private TaskAssignment MapToDomainModel(TaskAssignmentDataModel dataModel)
        {
            var assignment = new TaskAssignment(
                dataModel.Id,
                dataModel.TaskId,
                dataModel.VolunteerId,
                dataModel.AssignedBy,
                dataModel.AssignmentSource, // Note: The domain entity might be using Source
                dataModel.Status,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt
            );
            return assignment;
        }

        public async Task<TaskAssignment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.TaskAssignments
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return dataModel == null ? null : MapToDomainModel(dataModel);
        }

        public async Task<IEnumerable<TaskAssignment>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.TaskAssignments
                .Where(x => x.TaskId == taskId)
                .ToListAsync(cancellationToken);
            return dataModels.Select(MapToDomainModel);
        }

        public async Task<IEnumerable<TaskAssignment>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.TaskAssignments
                .Where(x => x.VolunteerId == volunteerId)
                .ToListAsync(cancellationToken);
            return dataModels.Select(MapToDomainModel);
        }

        public async Task AddAsync(TaskAssignment assignment, CancellationToken cancellationToken = default)
        {
            var dataModel = new TaskAssignmentDataModel
            {
                Id = assignment.Id,
                TaskId = assignment.TaskId,
                VolunteerId = assignment.VolunteerId,
                AssignedBy = assignment.AssignedBy,
                AssignmentSource = assignment.Source,
                Status = assignment.Status,
                CreatedAt = assignment.CreatedAt,
                UpdatedAt = assignment.UpdatedAt,
                DeletedAt = assignment.DeletedAt,
                AssignedAt = assignment.CreatedAt // Fallback since domain model lacks this property
            };
            await _context.TaskAssignments.AddAsync(dataModel, cancellationToken);
        }

        public async Task UpdateAsync(TaskAssignment assignment, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.TaskAssignments.FindAsync(new object[] { assignment.Id }, cancellationToken);
            if (dataModel == null) return;

            dataModel.Status = assignment.Status;
            dataModel.AssignmentSource = assignment.Source;
            dataModel.UpdatedAt = assignment.UpdatedAt;
            dataModel.DeletedAt = assignment.DeletedAt;

            _context.TaskAssignments.Update(dataModel);
        }

        public async Task DeleteAsync(TaskAssignment assignment, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.TaskAssignments.FindAsync(new object[] { assignment.Id }, cancellationToken);
            if (dataModel != null)
            {
                _context.TaskAssignments.Remove(dataModel);
            }
        }
    }
}
