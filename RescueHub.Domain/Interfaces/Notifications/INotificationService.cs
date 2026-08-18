using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;

namespace RescueHub.Domain.Interfaces.Notifications
{
    public interface INotificationService
    {
        // Lấy danh sách thông báo phân trang theo userId
        Task<PagedResult<Notification>> GetPagedByUserIdAsync(
            Guid userId,
            QueryCriteria criteria,
            CancellationToken cancellationToken);

        // Lấy thông báo theo Id (thuộc về userId)
        Task<Notification?> GetByIdAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken);

        // Đánh dấu một thông báo đã đọc
        Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken);

        // Đánh dấu tất cả thông báo của userId đã đọc
        Task<int> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Xóa một thông báo (thuộc về userId)
        Task<bool> DeleteAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken);

        // Xóa tất cả thông báo của userId
        Task<int> DeleteAllAsync(
            Guid userId,
            CancellationToken cancellationToken);

        // Tạo mới thông báo
        Task<Notification> CreateAsync(
            Notification notification,
            CancellationToken cancellationToken);
    }
}
