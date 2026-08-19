using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Commands
{
    // Xóa một thông báo theo Id của người dùng hiện tại
    public record DeleteNotificationCommand(
        Guid NotificationId,
        Guid UserId
    ) : IRequest<bool>;

    public class DeleteNotificationCommandHandler
        : IRequestHandler<DeleteNotificationCommand, bool>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNotificationCommandHandler(
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            DeleteNotificationCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var success = await _notificationService.DeleteAsync(
                    request.NotificationId,
                    request.UserId,
                    cancellationToken);

                if (!success)
                {
                    await _unitOfWork.RollbackAsync(cancellationToken);
                    return false;
                }

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
}
