using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Domain.Services
{
    public class ReliefRequestService : IReliefRequestService
    {
        private readonly IReliefRequestRepository _reliefRequestRepository;

        public ReliefRequestService(IReliefRequestRepository reliefRequestRepository)
        {
            _reliefRequestRepository = reliefRequestRepository;
        }

        public async Task<ReliefRequest> CreateReliefRequestAsync(
            Guid requesterId,
            GeoLocation location,
            string title,
            string description,
            string? reliefImageUrl,
            string? requestedResource,
            int urgencyLevel,
            int estimatedAffectedPeople,
            decimal? estimatedAffectedRadiusKm,
            CancellationToken cancellationToken)
        {
            var request = new ReliefRequest(
                Guid.NewGuid(),
                requesterId,
                location,
                title,
                description,
                reliefImageUrl,
                requestedResource,
                urgencyLevel,
                estimatedAffectedPeople,
                estimatedAffectedRadiusKm,
                ReliefRequestStatus.Pending,
                DateTime.UtcNow,
                null
            );
            return await _reliefRequestRepository.AddAsync(request, cancellationToken);
        }

        public async Task<ReliefRequest?> UpdateReliefRequestAsync(
            Guid requestId,
            string title,
            string description,
            string? reliefImageUrl,
            string? requestedResource,
            int urgencyLevel,
            int estimatedAffectedPeople,
            decimal? estimatedAffectedRadiusKm,
            GeoLocation location,
            CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return null;

            request.UpdateDetails(title, description, reliefImageUrl, requestedResource,
                urgencyLevel, estimatedAffectedPeople, estimatedAffectedRadiusKm, location);

            await _reliefRequestRepository.UpdateAsync(request, cancellationToken);
            return request;
        }

        public async Task<bool> ApproveReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return false;

            request.Approve(coordinatorId); // throws InvalidOperationException if wrong status
            await _reliefRequestRepository.UpdateAsync(request, cancellationToken);
            return true;
        }

        public async Task<bool> RejectReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return false;

            request.Reject(coordinatorId); // throws InvalidOperationException if wrong status
            await _reliefRequestRepository.UpdateAsync(request, cancellationToken);
            return true;
        }

        public async Task<bool> CompleteReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return false;

            request.Complete(coordinatorId); // throws InvalidOperationException if wrong status
            await _reliefRequestRepository.UpdateAsync(request, cancellationToken);
            return true;
        }

        public async Task<bool> CancelReliefRequestAsync(Guid requestId, Guid requesterId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null || request.RequesterId != requesterId) return false;

            request.Cancel(); // throws InvalidOperationException if wrong status
            await _reliefRequestRepository.UpdateAsync(request, cancellationToken);
            return true;
        }

        public async Task<bool> ReportReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return false;
            // Report is a read/export action – no state change required
            return true;
        }

        public async Task<bool> ExportReliefRequestAsync(Guid requestId, Guid coordinatorId, CancellationToken cancellationToken)
        {
            var request = await _reliefRequestRepository.GetByIdAsync(requestId, cancellationToken);
            if (request == null) return false;
            // Export is a read/export action – no state change required
            return true;
        }
    }
}
