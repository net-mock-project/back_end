using MediatR;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.VolunteerEngagements.Commands;

public record CancelAvailabilityCommand(Guid VolunteerId, Guid RequestId) : IRequest<bool>;

public class CancelAvailabilityCommandHandler : IRequestHandler<CancelAvailabilityCommand, bool>
{
    private readonly IReliefTaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelAvailabilityCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelAvailabilityCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await _taskService.CancelAvailabilityAsync(request.VolunteerId, request.RequestId, cancellationToken);
            if (result)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            return result;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
