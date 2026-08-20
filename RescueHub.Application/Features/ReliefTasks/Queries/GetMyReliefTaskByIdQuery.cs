using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Queries
{
    public record GetMyReliefTaskByIdQuery(Guid VolunteerId, Guid TaskId) : IRequest<ReliefTaskDto?>;

    public class GetMyReliefTaskByIdQueryHandler : IRequestHandler<GetMyReliefTaskByIdQuery, ReliefTaskDto?>
    {
        private readonly ITaskAssignmentRepository _assignmentRepository;
        private readonly IReliefTaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetMyReliefTaskByIdQueryHandler(
            ITaskAssignmentRepository assignmentRepository,
            IReliefTaskRepository taskRepository,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<ReliefTaskDto?> Handle(GetMyReliefTaskByIdQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _assignmentRepository.GetByVolunteerIdAsync(request.VolunteerId, cancellationToken);
            if (!assignments.Any(a => a.TaskId == request.TaskId))
            {
                return null;
            }

            var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
            if (task == null)
            {
                return null;
            }

            return _mapper.Map<ReliefTaskDto>(task);
        }
    }
}
