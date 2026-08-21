using MediatR;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Commands;

public record CompleteReliefTaskCommand(Guid Id) : IRequest<bool>;

public class CompleteReliefTaskCommandHandler : IRequestHandler<CompleteReliefTaskCommand, bool>
{
    private readonly IReliefTaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteReliefTaskCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CompleteReliefTaskCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var task = await _taskService.CompleteTaskAsync(request.Id, cancellationToken);
            if (task == null) return false;
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
