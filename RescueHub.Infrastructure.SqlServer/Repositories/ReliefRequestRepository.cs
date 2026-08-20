using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.ReliefRequests;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class ReliefRequestRepository : IReliefRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ReliefRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        private ReliefRequest MapToDomainModel(ReliefRequestDataModel dataModel)
        {
            var location = new RescueHub.Domain.Common.Enums.GeoLocation(dataModel.Location.Y, dataModel.Location.X);
            var request = new ReliefRequest(
                dataModel.Id,
                dataModel.RequesterId,
                location,
                dataModel.Title,
                dataModel.Description,
                dataModel.ReliefImageUrl,
                dataModel.RequestedResource,
                dataModel.UrgencyLevel,
                dataModel.EstimatedAffectedPeople,
                dataModel.EstimatedAffectedRadiusKm,
                dataModel.Status,
                dataModel.CreatedAt,
                dataModel.UpdatedAt,
                dataModel.DeletedAt
            );

            // Restore private-set fields from DB via backing field reflection
            SetField(request, "<CoordinatorId>k__BackingField", dataModel.CoordinatorId);
            SetField(request, "<StartTime>k__BackingField", dataModel.StartTime);
            SetField(request, "<EndTime>k__BackingField", dataModel.EndTime);
            SetField(request, "<CompletedAt>k__BackingField", dataModel.CompletedAt);

            return request;
        }

        private static void SetField(object target, string fieldName, object? value)
        {
            var field = target.GetType()
                .GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            field?.SetValue(target, value);
        }

        public async Task<ReliefRequest> AddAsync(ReliefRequest request, CancellationToken cancellationToken)
        {
            var point = new NetTopologySuite.Geometries.Point(request.Location.Longitude, request.Location.Latitude) { SRID = 4326 };
            var dataModel = new ReliefRequestDataModel
            {
                Id = request.Id,
                RequesterId = request.RequesterId,
                CoordinatorId = request.CoordinatorId,
                Location = point,
                Title = request.Title,
                Description = request.Description,
                ReliefImageUrl = request.ReliefImageUrl,
                RequestedResource = request.RequestedResource,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                UrgencyLevel = request.UrgencyLevel,
                EstimatedAffectedPeople = request.EstimatedAffectedPeople,
                EstimatedAffectedRadiusKm = request.EstimatedAffectedRadiusKm,
                Status = request.Status,
                CompletedAt = request.CompletedAt,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                DeletedAt = request.DeletedAt
            };
            await _context.ReliefRequests.AddAsync(dataModel, cancellationToken);
            // SaveChanges is handled by IUnitOfWork in the Application layer
            return request;
        }

        public async Task<ReliefRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var dataModel = await _context.ReliefRequests
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return dataModel == null ? null : MapToDomainModel(dataModel);
        }

        public async Task<List<ReliefRequest>> GetAllAsync(CancellationToken cancellationToken)
        {
            var dataModels = await _context.ReliefRequests
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return dataModels.Select(MapToDomainModel).ToList();
        }

        public async Task<List<ReliefRequest>> GetByRequesterIdAsync(Guid requesterId, CancellationToken cancellationToken)
        {
            var dataModels = await _context.ReliefRequests
                .Where(x => x.RequesterId == requesterId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken);

            return dataModels.Select(MapToDomainModel).ToList();
        }

        public async Task UpdateAsync(ReliefRequest request, CancellationToken cancellationToken)
        {
            var dataModel = await _context.ReliefRequests.FindAsync(new object[] { request.Id }, cancellationToken);
            if (dataModel == null) return;

            var point = new NetTopologySuite.Geometries.Point(request.Location.Longitude, request.Location.Latitude) { SRID = 4326 };
            dataModel.CoordinatorId = request.CoordinatorId;
            dataModel.Location = point;
            dataModel.Title = request.Title;
            dataModel.Description = request.Description;
            dataModel.ReliefImageUrl = request.ReliefImageUrl;
            dataModel.RequestedResource = request.RequestedResource;
            dataModel.StartTime = request.StartTime;
            dataModel.EndTime = request.EndTime;
            dataModel.UrgencyLevel = request.UrgencyLevel;
            dataModel.EstimatedAffectedPeople = request.EstimatedAffectedPeople;
            dataModel.EstimatedAffectedRadiusKm = request.EstimatedAffectedRadiusKm;
            dataModel.Status = request.Status;
            dataModel.CompletedAt = request.CompletedAt;
            dataModel.UpdatedAt = request.UpdatedAt;
            dataModel.DeletedAt = request.DeletedAt;

            _context.ReliefRequests.Update(dataModel);
            // SaveChanges is handled by IUnitOfWork in the Application layer
        }

        public async Task DeleteAsync(ReliefRequest request, CancellationToken cancellationToken)
        {
            var dataModel = await _context.ReliefRequests.FindAsync(new object[] { request.Id }, cancellationToken);
            if (dataModel != null)
            {
                _context.ReliefRequests.Remove(dataModel);
                // SaveChanges is handled by IUnitOfWork in the Application layer
            }
        }
    }
}
