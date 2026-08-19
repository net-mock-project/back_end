using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RescueHub.API.Models.Notifications;
using RescueHub.Application.Common.Exceptions;
using RescueHub.Application.Contracts.Querying;
using RescueHub.Application.Features.Notifications.Commands;
using RescueHub.Application.Features.Notifications.Queries;
using RescueHub.Domain.Common.Querying;
using System.Security.Claims;

namespace RescueHub.API.Controllers
{
    [ApiController]
    [Route("api/me/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IMapper _mapper;

        public NotificationsController(ISender sender, IMapper mapper)
        {
            _sender = sender;
            _mapper = mapper;
        }

        // Lấy danh sách thông báo của người dùng hiện tại
        [HttpGet]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] NotificationQueryRequest request,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var queryRequest = _mapper.Map<QueryRequest>(request);
            var criteria = _mapper.Map<QueryCriteria>(queryRequest);

            var result = await _sender.Send(
                new GetNotificationsQuery(userId.Value, criteria),
                cancellationToken);

            return Ok(result);
        }

        // Xem chi tiết một thông báo
        [HttpGet("{notificationId:guid}")]
        public async Task<IActionResult> GetNotificationById(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var result = await _sender.Send(
                new GetNotificationByIdQuery(notificationId, userId.Value),
                cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(
                    $"Notification '{notificationId}' not found.");
            }

            return Ok(result);
        }

        // Đánh dấu một thông báo đã đọc
        [HttpPatch("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkAsRead(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var success = await _sender.Send(
                new MarkNotificationAsReadCommand(notificationId, userId.Value),
                cancellationToken);

            if (!success)
            {
                throw new NotFoundException(
                    $"Notification '{notificationId}' not found.");
            }

            return NoContent();
        }

        // Đánh dấu tất cả thông báo đã đọc
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var updatedCount = await _sender.Send(
                new MarkAllNotificationsAsReadCommand(userId.Value),
                cancellationToken);

            return Ok(new { updatedCount });
        }

        // Xóa một thông báo
        [HttpDelete("{notificationId:guid}")]
        public async Task<IActionResult> DeleteNotification(
            Guid notificationId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var success = await _sender.Send(
                new DeleteNotificationCommand(notificationId, userId.Value),
                cancellationToken);

            if (!success)
            {
                throw new NotFoundException(
                    $"Notification '{notificationId}' not found.");
            }

            return NoContent();
        }

        // Xóa tất cả thông báo
        [HttpDelete]
        public async Task<IActionResult> DeleteAllNotifications(
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return Unauthorized();

            var deletedCount = await _sender.Send(
                new DeleteAllNotificationsCommand(userId.Value),
                cancellationToken);

            return Ok(new { deletedCount });
        }

        // Lấy UserId từ token đăng nhập
        private Guid? GetCurrentUserId()
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(userIdValue, out var userId)
                ? userId
                : null;
        }
    }
}
