using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Queries
{
    public record GetReliefTaskByIdQuery(Guid RequestId, Guid TaskId) : IRequest<ReliefTaskDto?>;

    public class GetReliefTaskByIdQueryHandler : IRequestHandler<GetReliefTaskByIdQuery, ReliefTaskDto?>
    {
        private readonly IReliefTaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetReliefTaskByIdQueryHandler(IReliefTaskRepository taskRepository, IMapper mapper)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<ReliefTaskDto?> Handle(GetReliefTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
            if (task == null || task.RequestId != request.RequestId)
            {
                return null;
            }
            
            return _mapper.Map<ReliefTaskDto>(task);
        }
    }
}
