using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Commands
{
    // Đánh dấu tất cả thông báo của người dùng là đã đọc
    public record MarkAllNotificationsAsReadCommand(
        Guid UserId
    ) : IRequest<int>;

    public class MarkAllNotificationsAsReadCommandHandler
        : IRequestHandler<MarkAllNotificationsAsReadCommand, int>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public MarkAllNotificationsAsReadCommandHandler(
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(
            MarkAllNotificationsAsReadCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var updatedCount = await _notificationService.MarkAllAsReadAsync(
                    request.UserId,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return updatedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
