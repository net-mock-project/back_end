using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Interfaces.ReliefRequests;
using RescueHub.Application.Common.Exceptions;

namespace RescueHub.Application.Features.ReliefRequests.Queries
{
    public record GetReliefRequestByIdQuery(Guid RequestId, Guid? UserId = null, bool IsCoordinator = false) : IRequest<ReliefRequestDto>;

    public class GetReliefRequestByIdQueryHandler : IRequestHandler<GetReliefRequestByIdQuery, ReliefRequestDto>
    {
        private readonly IReliefRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetReliefRequestByIdQueryHandler(IReliefRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ReliefRequestDto> Handle(GetReliefRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.RequestId, cancellationToken);
            if (entity == null) throw new NotFoundException($"ReliefRequest {request.RequestId} not found");

            if (!request.IsCoordinator && request.UserId.HasValue && entity.RequesterId != request.UserId.Value)
            {
                throw new UnauthorizedAccessException();
            }

            return _mapper.Map<ReliefRequestDto>(entity);
        }
    }
}
