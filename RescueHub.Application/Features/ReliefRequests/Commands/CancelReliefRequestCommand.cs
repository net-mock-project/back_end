using MediatR;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Commands
{
    public record CancelReliefRequestCommand(Guid RequestId, Guid RequesterId) : IRequest<bool>;

    public class CancelReliefRequestCommandHandler : IRequestHandler<CancelReliefRequestCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReliefRequestService _service;
        private readonly IReliefRequestRepository _repository;

        public CancelReliefRequestCommandHandler(
            IUnitOfWork unitOfWork,
            IReliefRequestService service,
            IReliefRequestRepository repository)
        {
            _unitOfWork = unitOfWork;
            _service = service;
            _repository = repository;
        }

        public async Task<bool> Handle(CancelReliefRequestCommand request, CancellationToken cancellationToken)
        {
            var existing = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (existing == null) throw new NotFoundException($"ReliefRequest {request.RequestId} not found");
            if (existing.RequesterId != request.RequesterId) throw new UnauthorizedAccessException();
            if (existing.Status != ReliefRequestStatus.Pending)
                throw new InvalidOperationException("Chỉ có thể hủy yêu cầu đang chờ duyệt.");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await _service.CancelReliefRequestAsync(
                    request.RequestId, request.RequesterId, cancellationToken);

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
