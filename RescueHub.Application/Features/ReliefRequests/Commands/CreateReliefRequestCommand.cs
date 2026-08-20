using Mapster;
using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Commands
{
    public record CreateReliefRequestCommand(
        Guid RequesterId,
        double Longitude,
        double Latitude,
        string Title,
        string Description,
        string? ReliefImageUrl,
        string? RequestedResource,
        int UrgencyLevel,
        int EstimatedAffectedPeople,
        decimal? EstimatedAffectedRadiusKm) : IRequest<ReliefRequestDto>;

    public class CreateReliefRequestCommandHandler : IRequestHandler<CreateReliefRequestCommand, ReliefRequestDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReliefRequestService _service;

        public CreateReliefRequestCommandHandler(IUnitOfWork unitOfWork, IReliefRequestService service)
        {
            _unitOfWork = unitOfWork;
            _service = service;
        }

        public async Task<ReliefRequestDto> Handle(CreateReliefRequestCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var location = new RescueHub.Domain.Common.Enums.GeoLocation(request.Latitude, request.Longitude);
                var entity = await _service.CreateReliefRequestAsync(
                    request.RequesterId,
                    location,
                    request.Title,
                    request.Description,
                    request.ReliefImageUrl,
                    request.RequestedResource,
                    request.UrgencyLevel,
                    request.EstimatedAffectedPeople,
                    request.EstimatedAffectedRadiusKm,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return entity.Adapt<ReliefRequestDto>();
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
