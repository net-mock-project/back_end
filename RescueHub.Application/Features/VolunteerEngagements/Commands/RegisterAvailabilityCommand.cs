using MediatR;
using RescueHub.Application.Contracts.VolunteerEngagements;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.VolunteerEngagements.Commands;

public record RegisterAvailabilityCommand(Guid VolunteerId, Guid RequestId) : IRequest<VolunteerEngagementDto?>;

public class RegisterAvailabilityCommandHandler : IRequestHandler<RegisterAvailabilityCommand, VolunteerEngagementDto?>
{
    private readonly IReliefTaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterAvailabilityCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VolunteerEngagementDto?> Handle(RegisterAvailabilityCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var engagement = await _taskService.RegisterAvailabilityAsync(request.VolunteerId, request.RequestId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new VolunteerEngagementDto(
                engagement.Id,
                engagement.VolunteerId,
                engagement.RequestId,
                engagement.Status,
                engagement.CreatedAt,
                engagement.UpdatedAt
            );
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
