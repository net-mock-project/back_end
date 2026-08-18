using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Queries
{
    // Lấy chi tiết một thông báo theo Id của người dùng hiện tại
    public record GetNotificationByIdQuery(
        Guid NotificationId,
        Guid UserId
    ) : IRequest<NotificationDto?>;

    public class GetNotificationByIdQueryHandler
        : IRequestHandler<GetNotificationByIdQuery, NotificationDto?>
    {
        private readonly INotificationService _notificationService;

        public GetNotificationByIdQueryHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<NotificationDto?> Handle(
            GetNotificationByIdQuery request,
            CancellationToken cancellationToken)
        {
            var notification = await _notificationService.GetByIdAsync(
                request.NotificationId,
                request.UserId,
                cancellationToken);

            return notification?.Adapt<NotificationDto>();
        }
    }
}
