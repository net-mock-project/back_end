using RescueHub.Domain.Entities;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Interfaces.ReliefRequests
{
    public interface IReliefRequestService
    {
        Task<ReliefRequest> CreateReliefRequestAsync(Guid requesterId, GeoLocation location, string title, string description, string? reliefImageUrl, string? requestedResource, int urgencyLevel, int estimatedAffectedPeople, decimal? estimatedAffectedRadiusKm, CancellationToken cancellationToken);
        Task<ReliefRequest?> UpdateReliefRequestAsync(Guid requestId, string title, string description, string? reliefImageUrl, string? requestedResource, int urgencyLevel, int estimatedAffectedPeople, decimal? estimatedAffectedRadiusKm, GeoLocation location, CancellationToken cancellationToken);
        Task<bool> ApproveReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<bool> RejectReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<bool> CompleteReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<bool> CancelReliefRequestAsync(Guid requestId, Guid requesterId, CancellationToken cancellationToken);
        Task<bool> ReportReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken);
        Task<bool> ExportReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken);
    }
}
