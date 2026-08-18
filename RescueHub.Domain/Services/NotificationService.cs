using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Notifications;

namespace RescueHub.Domain.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public NotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public Task<PagedResult<Notification>> GetPagedByUserIdAsync(
            Guid userId,
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.GetPagedByUserIdAsync(
                userId, criteria, cancellationToken);
        }

        public Task<Notification?> GetByIdAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.GetByIdAsync(
                notificationId, userId, cancellationToken);
        }

        public Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.MarkAsReadAsync(
                notificationId, userId, cancellationToken);
        }

        public Task<int> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.MarkAllAsReadAsync(
                userId, cancellationToken);
        }

        public Task<bool> DeleteAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.DeleteAsync(
                notificationId, userId, cancellationToken);
        }

        public Task<int> DeleteAllAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.DeleteAllAsync(
                userId, cancellationToken);
        }

        public Task<Notification> CreateAsync(
            Notification notification,
            CancellationToken cancellationToken)
        {
            return _notificationRepository.CreateAsync(
                notification, cancellationToken);
        }
    }
}
