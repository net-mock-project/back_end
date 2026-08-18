using Mapster;
using MediatR;
using RescueHub.Application.Contracts.Notifications;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Application.Features.Notifications.Queries
{
    // Lấy danh sách thông báo phân trang của người dùng hiện tại
    public record GetNotificationsQuery(
        Guid UserId,
        QueryCriteria Criteria
    ) : IRequest<PaginationResponse<NotificationDto>>;

    public class GetNotificationsQueryHandler
        : IRequestHandler<
            GetNotificationsQuery,
            PaginationResponse<NotificationDto>>
    {
        private readonly INotificationService _notificationService;

        public GetNotificationsQueryHandler(
            INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<PaginationResponse<NotificationDto>> Handle(
            GetNotificationsQuery request,
            CancellationToken cancellationToken)
        {
            var result = await _notificationService.GetPagedByUserIdAsync(
                request.UserId,
                request.Criteria,
                cancellationToken);

            var items = result.Items
                .Select(x => x.Adapt<NotificationDto>())
                .ToList();

            return new PaginationResponse<NotificationDto>(
                items,
                result.TotalCount,
                request.Criteria.PageNumber,
                request.Criteria.PageSize);
        }
    }
}
