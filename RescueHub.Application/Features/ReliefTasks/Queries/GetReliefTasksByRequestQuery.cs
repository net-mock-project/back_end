using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Queries
{
    public record GetReliefTasksByRequestQuery(Guid RequestId) : IRequest<IEnumerable<ReliefTaskDto>>;

    public class GetReliefTasksByRequestQueryHandler : IRequestHandler<GetReliefTasksByRequestQuery, IEnumerable<ReliefTaskDto>>
    {
        private readonly IReliefTaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetReliefTasksByRequestQueryHandler(IReliefTaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReliefTaskDto>> Handle(GetReliefTasksByRequestQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetByRequestIdAsync(request.RequestId, cancellationToken);
            return _mapper.Map<IEnumerable<ReliefTaskDto>>(tasks);
        }
    }
}
