using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class ReliefTaskRepository : IReliefTaskRepository
    {
        private readonly ApplicationDbContext _context;

        public ReliefTaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private ReliefTask MapToDomainModel(ReliefTaskDataModel dataModel)
        {
            var location = dataModel.Location != null 
                ? new RescueHub.Domain.Common.Enums.GeoLocation(dataModel.Location.Y, dataModel.Location.X) 
                : null;
            
            var taskSkills = dataModel.TaskSkills?.Select(ts => ts.SkillId).ToList() ?? new List<Guid>();

            var task = new ReliefTask(
                dataModel.Id,
                dataModel.RequestId,
                dataModel.Title,
                dataModel.Description ?? string.Empty,
                dataModel.RequiredVolunteers,
                dataModel.Priority,
                location,
                dataModel.Status,
                taskSkills,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt
            );

            return task;
        }

        public async Task<ReliefTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.ReliefTasks
                .Include(x => x.TaskSkills)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return dataModel == null ? null : MapToDomainModel(dataModel);
        }

        public async Task<IEnumerable<ReliefTask>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.ReliefTasks
                .Include(x => x.TaskSkills)
                .Where(x => x.RequestId == requestId)
                .ToListAsync(cancellationToken);

            return dataModels.Select(MapToDomainModel);
        }

        public async Task<IEnumerable<ReliefTask>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.ReliefTasks
                .Include(x => x.TaskSkills)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);

            return dataModels.Select(MapToDomainModel);
        }

        public async Task AddAsync(ReliefTask task, CancellationToken cancellationToken = default)
        {
            NetTopologySuite.Geometries.Point? point = null;
            if (task.Location != null)
            {
                point = new NetTopologySuite.Geometries.Point(task.Location.Longitude, task.Location.Latitude) { SRID = 4326 };
            }

            var dataModel = new ReliefTaskDataModel
            {
                Id = task.Id,
                RequestId = task.RequestId,
                Title = task.Title,
                Description = task.Description,
                RequiredVolunteers = task.RequiredVolunteers,
                Priority = task.Priority,
                Location = point,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt,
                DeletedAt = task.DeletedAt,
                TaskSkills = task.TaskSkills.Select(ts => new TaskSkillDataModel { TaskId = task.Id, SkillId = ts }).ToList()
            };

            await _context.ReliefTasks.AddAsync(dataModel, cancellationToken);
        }

        public async Task UpdateAsync(ReliefTask task, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.ReliefTasks
                .Include(x => x.TaskSkills)
                .FirstOrDefaultAsync(x => x.Id == task.Id, cancellationToken);
            
            if (dataModel == null) return;

            NetTopologySuite.Geometries.Point? point = null;
            if (task.Location != null)
            {
                point = new NetTopologySuite.Geometries.Point(task.Location.Longitude, task.Location.Latitude) { SRID = 4326 };
            }

            dataModel.RequestId = task.RequestId;
            dataModel.Title = task.Title;
            dataModel.Description = task.Description;
            dataModel.RequiredVolunteers = task.RequiredVolunteers;
            dataModel.Priority = task.Priority;
            dataModel.Location = point;
            dataModel.Status = task.Status;
            dataModel.UpdatedAt = task.UpdatedAt;
            dataModel.DeletedAt = task.DeletedAt;

            // Update TaskSkills
            _context.TaskSkills.RemoveRange(dataModel.TaskSkills);
            dataModel.TaskSkills = task.TaskSkills.Select(ts => new TaskSkillDataModel { TaskId = task.Id, SkillId = ts }).ToList();

            _context.ReliefTasks.Update(dataModel);
        }

        public async Task DeleteAsync(ReliefTask task, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.ReliefTasks
                .Include(x => x.TaskSkills)
                .Include(x => x.Assignments)
                .FirstOrDefaultAsync(x => x.Id == task.Id, cancellationToken);
                
            if (dataModel != null)
            {
                if (dataModel.TaskSkills != null && dataModel.TaskSkills.Any())
                {
                    _context.TaskSkills.RemoveRange(dataModel.TaskSkills);
                }
                
                if (dataModel.Assignments != null && dataModel.Assignments.Any())
                {
                    _context.TaskAssignments.RemoveRange(dataModel.Assignments);
                }

                _context.ReliefTasks.Remove(dataModel);
            }
        }
    }
}
