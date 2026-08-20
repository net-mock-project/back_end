using Mapster;
using MediatR;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Commands
{
    public record UpdateReliefRequestCommand(
        Guid RequestId,
        Guid UserId,
        bool IsCoordinator,
        double Longitude,
        double Latitude,
        string Title,
        string Description,
        string? ReliefImageUrl,
        string? RequestedResource,
        int UrgencyLevel,
        int EstimatedAffectedPeople,
        decimal? EstimatedAffectedRadiusKm) : IRequest<ReliefRequestDto>;

    public class UpdateReliefRequestCommandHandler : IRequestHandler<UpdateReliefRequestCommand, ReliefRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReliefRequestService _service;
        private readonly IReliefRequestRepository _repository;

        public UpdateReliefRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IReliefRequestService service,
            IReliefRequestRepository repository)
        {
            _unitOfWork = unitOfWork;
            _service = service;
            _repository = repository;
        }

        public async Task<ReliefRequestDto> Handle(UpdateReliefRequestCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (existing == null) throw new NotFoundException($"ReliefRequest {request.RequestId} not found");

            if (!request.IsCoordinator)
            {
                if (existing.RequesterId != request.UserId)
                    throw new UnauthorizedAccessException();
                if (existing.Status != ReliefRequestStatus.Pending)
                    throw new InvalidOperationException("Chỉ có thể sửa yêu cầu đang chờ duyệt.");
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var location = new GeoLocation(request.Latitude, request.Longitude);
                var entity = await _service.UpdateReliefRequestAsync(
                    request.RequestId,
                    request.Title,
                    request.Description,
                    request.ReliefImageUrl,
                    request.RequestedResource,
                    request.UrgencyLevel,
                    request.EstimatedAffectedPeople,
                    request.EstimatedAffectedRadiusKm,
                    location,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return entity!.Adapt<ReliefRequestDto>();
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
