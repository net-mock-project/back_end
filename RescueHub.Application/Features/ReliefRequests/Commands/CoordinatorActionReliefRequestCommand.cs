using MediatR;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Commands
{
    public record CoordinatorActionReliefRequestCommand(Guid RequestId, Guid CoordinatorId, string Action) : IRequest<bool>;

    public class CoordinatorActionReliefRequestCommandHandler : IRequestHandler<CoordinatorActionReliefRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReliefRequestService _service;
        private readonly IReliefRequestRepository _repository;

        public CoordinatorActionReliefRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IReliefRequestService service,
            IReliefRequestRepository repository)
        {
            _unitOfWork = unitOfWork;
            _service = service;
            _repository = repository;
        }

        public async Task<bool> Handle(CoordinatorActionReliefRequestCommand request, CancellationToken cancellationToken)
        {
            // Validate existence and status before mutating
            var existing = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (existing == null)
                throw new NotFoundException($"ReliefRequest {request.RequestId} not found");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = request.Action.ToLower() switch
                {
                    "approve" => await _service.ApproveReliefRequestAsync(request.RequestId, request.CoordinatorId, cancellationToken),
                    "reject"  => await _service.RejectReliefRequestAsync(request.RequestId, request.CoordinatorId, cancellationToken),
                    "complete" => await _service.CompleteReliefRequestAsync(request.RequestId, request.CoordinatorId, cancellationToken),
                    "report"  => await _service.ReportReliefRequestAsync(request.RequestId, request.CoordinatorId, cancellationToken),
                    "export"  => await _service.ExportReliefRequestAsync(request.RequestId, request.CoordinatorId, cancellationToken),
                    _ => throw new InvalidOperationException($"Invalid action: {request.Action}")
                };

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
