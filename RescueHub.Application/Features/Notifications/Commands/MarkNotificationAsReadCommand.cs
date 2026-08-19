using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Commands
{
    // Đánh dấu một thông báo đã đọc
    public record MarkNotificationAsReadCommand(
        Guid NotificationId,
        Guid UserId
    ) : IRequest<bool>;

    public class MarkNotificationAsReadCommandHandler
        : IRequestHandler<MarkNotificationAsReadCommand, bool>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public MarkNotificationAsReadCommandHandler(
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            MarkNotificationAsReadCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var success = await _notificationService.MarkAsReadAsync(
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
