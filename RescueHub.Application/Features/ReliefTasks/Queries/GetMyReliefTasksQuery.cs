using MapsterMapper;
using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Queries
{
    public record GetMyReliefTasksQuery(Guid VolunteerId) : IRequest<IEnumerable<ReliefTaskDto>>;

    public class GetMyReliefTasksQueryHandler : IRequestHandler<GetMyReliefTasksQuery, IEnumerable<ReliefTaskDto>>
    {
        private readonly ITaskAssignmentRepository _assignmentRepository;
        private readonly IReliefTaskRepository _taskRepository;
        private readonly IMapper _mapper;

        public GetMyReliefTasksQueryHandler(
            ITaskAssignmentRepository assignmentRepository,
            IReliefTaskRepository taskRepository,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _taskRepository = taskRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ReliefTaskDto>> Handle(GetMyReliefTasksQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _assignmentRepository.GetByVolunteerIdAsync(request.VolunteerId, cancellationToken);
            var taskIds = assignments.Select(a => a.TaskId).ToList();

            if (!taskIds.Any())
            {
                return new List<ReliefTaskDto>();
            }

            var tasks = await _taskRepository.GetByIdsAsync(taskIds, cancellationToken);
            return _mapper.Map<IEnumerable<ReliefTaskDto>>(tasks);
        }
    }
}
