using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefRequests;
using RescueHub.Domain.Interfaces.ReliefRequests;

namespace RescueHub.Application.Features.ReliefRequests.Queries
{
    public record GetAllReliefRequestsQuery() : IRequest<List<ReliefRequestDto>>;

    public class GetAllReliefRequestsQueryHandler : IRequestHandler<GetAllReliefRequestsQuery, List<ReliefRequestDto>>
    {
        private readonly IReliefRequestRepository _repository;
        private readonly IMapper _mapper;

        public GetAllReliefRequestsQueryHandler(IReliefRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<ReliefRequestDto>> Handle(GetAllReliefRequestsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<ReliefRequestDto>>(entities);
        }
    }
}
