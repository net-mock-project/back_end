using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class VolunteerEngagementRepository : IVolunteerEngagementRepository
    {
        private readonly ApplicationDbContext _context;

        public VolunteerEngagementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private VolunteerEngagement MapToDomainModel(VolunteerEngagementDataModel dataModel)
        {
            var engagement = new VolunteerEngagement(
                dataModel.Id,
                dataModel.VolunteerId,
                dataModel.RequestId,
                dataModel.Status,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt
            );
            return engagement;
        }

        public async Task<VolunteerEngagement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.VolunteerEngagements
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            return dataModel == null ? null : MapToDomainModel(dataModel);
        }

        public async Task<VolunteerEngagement?> GetByVolunteerAndRequestAsync(Guid volunteerId, Guid requestId, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.VolunteerEngagements
                .FirstOrDefaultAsync(x => x.VolunteerId == volunteerId && x.RequestId == requestId, cancellationToken);
            return dataModel == null ? null : MapToDomainModel(dataModel);
        }

        public async Task<IEnumerable<VolunteerEngagement>> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.VolunteerEngagements
                .Where(x => x.RequestId == requestId)
                .ToListAsync(cancellationToken);
            return dataModels.Select(MapToDomainModel);
        }

        public async Task<IEnumerable<VolunteerEngagement>> GetByVolunteerIdAsync(Guid volunteerId, CancellationToken cancellationToken = default)
        {
            var dataModels = await _context.VolunteerEngagements
                .Where(x => x.VolunteerId == volunteerId)
                .ToListAsync(cancellationToken);
            return dataModels.Select(MapToDomainModel);
        }

        public async Task AddAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default)
        {
            var dataModel = new VolunteerEngagementDataModel
            {
                Id = engagement.Id,
                VolunteerId = engagement.VolunteerId,
                RequestId = engagement.RequestId,
                Status = engagement.Status,
                CreatedAt = engagement.CreatedAt,
                UpdatedAt = engagement.UpdatedAt,
                DeletedAt = engagement.DeletedAt
            };
            await _context.VolunteerEngagements.AddAsync(dataModel, cancellationToken);
        }

        public async Task UpdateAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.VolunteerEngagements.FindAsync(new object[] { engagement.Id }, cancellationToken);
            if (dataModel == null) return;

            dataModel.Status = engagement.Status;
            dataModel.UpdatedAt = engagement.UpdatedAt;
            dataModel.DeletedAt = engagement.DeletedAt;

            _context.VolunteerEngagements.Update(dataModel);
        }

        public async Task DeleteAsync(VolunteerEngagement engagement, CancellationToken cancellationToken = default)
        {
            var dataModel = await _context.VolunteerEngagements.FindAsync(new object[] { engagement.Id }, cancellationToken);
            if (dataModel != null)
            {
                _context.VolunteerEngagements.Remove(dataModel);
            }
        }
    }
}
