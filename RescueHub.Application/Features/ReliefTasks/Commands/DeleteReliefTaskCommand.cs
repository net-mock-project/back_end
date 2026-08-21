using MediatR;
using RescueHub.Domain.Interfaces;
using RescueHub.Application.Common.Interfaces;

namespace RescueHub.Application.Features.ReliefTasks.Commands;

public record DeleteReliefTaskCommand(Guid Id) : IRequest<bool>;

public class DeleteReliefTaskCommandHandler : IRequestHandler<DeleteReliefTaskCommand, bool>
{
    private readonly IReliefTaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReliefTaskCommandHandler(IReliefTaskService taskService, IUnitOfWork unitOfWork)
    {
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteReliefTaskCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await _taskService.DeleteTaskAsync(request.Id, cancellationToken);
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
