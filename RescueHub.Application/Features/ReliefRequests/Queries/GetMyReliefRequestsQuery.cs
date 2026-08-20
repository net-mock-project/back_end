using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Queries
{
    public record GetMyReliefRequestsQuery(Guid UserId) : IRequest<List<ReliefRequestDto>>;

    public class GetMyReliefRequestsQueryHandler : IRequestHandler<GetMyReliefRequestsQuery, List<ReliefRequestDto>>
    {
        private readonly IReliefRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetMyReliefRequestsQueryHandler(IReliefRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ReliefRequestDto>> Handle(GetMyReliefRequestsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetByRequesterIdAsync(request.UserId, cancellationToken);
            return _mapper.Map<List<ReliefRequestDto>>(entities);
        }
    }
}
