using MediatR;
using RescueHub.Application.Contracts.ReliefTasks;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Commands;

public record CreateReliefTaskCommand(
    Guid RequestId,
    string Title,
    string Description,
    int RequiredVolunteers,
    int Priority,
    double? Latitude,
    double? Longitude,
    List<Guid> TaskSkills
) : IRequest<ReliefTaskDto>;

    public class CreateReliefTaskCommandHandler : IRequestHandler<CreateReliefTaskCommand, ReliefTaskDto>
    {
        private readonly IReliefTaskService _taskService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReliefTaskCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
        {
            _taskService = taskService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReliefTaskDto> Handle(CreateReliefTaskCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                GeoLocation? location = null;
                if (request.Latitude.HasValue && request.Longitude.HasValue)
                {
                    location = new GeoLocation(request.Latitude.Value, request.Longitude.Value);
                }

                var task = await _taskService.CreateTaskAsync(
                    request.RequestId,
                    request.Title,
                    request.Description,
                    request.RequiredVolunteers,
                    request.Priority,
                    location,
                    request.TaskSkills ?? new List<Guid>(),
                    cancellationToken
                );

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
