using MediatR;
using RescueHub.Application.Contracts.TaskAssignments;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.TaskAssignments.Commands;

public record AssignTaskCommand(
    Guid TaskId,
    Guid VolunteerId,
    Guid AssignedBy,
    bool IsInvite
) : IRequest<TaskAssignmentDto?>;

public class AssignTaskCommandHandler : IRequestHandler<AssignTaskCommand, TaskAssignmentDto?>
{
    private readonly IReliefTaskService _taskService;
    private readonly ITaskAssignmentRepository _assignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignTaskCommandHandler(IReliefTaskService taskService, ITaskAssignmentRepository assignmentRepository, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _assignmentRepository = assignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskAssignmentDto?> Handle(AssignTaskCommand request, CancellationToken cancellationToken)
    {
        var existingAssignments = await _assignmentRepository.GetByTaskIdAsync(request.TaskId, cancellationToken);
        if (existingAssignments.Any(a => a.VolunteerId == request.VolunteerId && a.Status != TaskAssignmentStatus.Cancelled && a.Status != TaskAssignmentStatus.Rejected))
        {
            return null; // Already assigned or invited
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            TaskAssignment assignment;
            if (request.IsInvite)
            {
                assignment = await _taskService.InviteVolunteerAsync(
                    request.TaskId,
                    request.VolunteerId,
                    request.AssignedBy,
                    cancellationToken
                );
            }
            else
            {
                assignment = await _taskService.AssignVolunteerAsync(
                    request.TaskId,
                    request.VolunteerId,
                    request.AssignedBy,
                    TaskAssignmentSource.Coordinator,
                    cancellationToken
                );
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new TaskAssignmentDto(
                assignment.Id,
                assignment.TaskId,
                assignment.VolunteerId,
                assignment.AssignedBy,
                assignment.Source,
                assignment.Status,
                assignment.CreatedAt,
                assignment.UpdatedAt
            );
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
