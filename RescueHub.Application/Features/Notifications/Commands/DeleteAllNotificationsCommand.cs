using MediatR;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Commands
{
    // Xóa tất cả thông báo của người dùng hiện tại
    public record DeleteAllNotificationsCommand(
        Guid UserId
    ) : IRequest<int>;

    public class DeleteAllNotificationsCommandHandler
        : IRequestHandler<DeleteAllNotificationsCommand, int>
    {
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteAllNotificationsCommandHandler(
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> Handle(
            DeleteAllNotificationsCommand request,
            CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var deletedCount = await _notificationService.DeleteAllAsync(
                    request.UserId,
                    cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                return deletedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
