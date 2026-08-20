using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Common.Exceptions;

namespace RescueHub.Application.Features.ReliefTasks.Commands;

public record UpdateReliefTaskCommand(
    Guid Id,
    string Title,
    string Description,
    int RequiredVolunteers,
    int Priority,
    double? Latitude,
    double? Longitude,
    List<Guid>? TaskSkills
) : IRequest<ReliefTaskDto>;

public class UpdateReliefTaskCommandHandler : IRequestHandler<UpdateReliefTaskCommand, ReliefTaskDto>
{
    private readonly IReliefTaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReliefTaskCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ReliefTaskDto> Handle(UpdateReliefTaskCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            GeoLocation? location = null;
            if (request.Latitude.HasValue && request.Longitude.HasValue)
            {
                location = new GeoLocation(request.Latitude.Value, request.Longitude.Value);
            }

            var task = await _taskService.UpdateTaskAsync(
                request.Id,
                request.Title,
                request.Description,
                request.RequiredVolunteers,
                request.Priority,
                location,
                request.TaskSkills ?? new List<Guid>(),
                cancellationToken
            );

            if (task == null)
                throw new NotFoundException($"ReliefTask with ID {request.Id} not found.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new ReliefTaskDto(
                task.Id,
                task.RequestId,
                task.Title,
                task.Description,
                task.RequiredVolunteers,
                task.Priority,
                task.Location?.Latitude,
                task.Location?.Longitude,
                task.Status,
                task.TaskSkills,
                task.CreatedAt,
                task.UpdatedAt
            );
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
