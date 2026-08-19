using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Common.Enums;
using RescueHub.Domain.Common.Querying;
using RescueHub.Domain.Entities;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Infrastructure.SqlServer.Models;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public NotificationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Notification>> GetPagedByUserIdAsync(
            Guid userId,
            QueryCriteria criteria,
            CancellationToken cancellationToken)
        {
            var query = _dbContext.Notifications
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .AsQueryable();

            query = ApplySearch(query, criteria.Search);
            query = ApplyFilters(query, criteria.Filters);
            query = ApplySorting(query, criteria.SortBy, criteria.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);

            var dataModels = await query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToListAsync(cancellationToken);

            var items = dataModels
                .Select(MapToDomain)
                .ToList();

            return new PagedResult<Notification>(items, totalCount);
        }

        public async Task<Notification?> GetByIdAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Notifications
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == notificationId && x.UserId == userId,
                    cancellationToken);

            return dataModel == null
                ? null
                : MapToDomain(dataModel);
        }

        public async Task<bool> MarkAsReadAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Notifications
                .FirstOrDefaultAsync(
                    x => x.Id == notificationId && x.UserId == userId,
                    cancellationToken);

            if (dataModel == null)
                return false;

            if (dataModel.IsRead)
                return true;

            dataModel.IsRead = true;
            return true;
        }

        public async Task<int> MarkAllAsReadAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var unreadNotifications = await _dbContext.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync(cancellationToken);

            if (unreadNotifications.Count == 0)
                return 0;

            foreach (var notification in unreadNotifications)
                notification.IsRead = true;

            return unreadNotifications.Count;
        }

        public async Task<bool> DeleteAsync(
            Guid notificationId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var dataModel = await _dbContext.Notifications
                .FirstOrDefaultAsync(
                    x => x.Id == notificationId && x.UserId == userId,
                    cancellationToken);

            if (dataModel == null)
                return false;

            _dbContext.Notifications.Remove(dataModel);
            return true;
        }

        public async Task<int> DeleteAllAsync(
            Guid userId,
            CancellationToken cancellationToken)
        {
            var notifications = await _dbContext.Notifications
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);

            if (notifications.Count == 0)
                return 0;

            _dbContext.Notifications.RemoveRange(notifications);
            return notifications.Count;
        }

        public Task<Notification> CreateAsync(
            Notification notification,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var dataModel = new NotificationDataModel
            {
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Content = notification.Content,
                Type = notification.Type,
                UrlLink = notification.UrlLink,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };

            _dbContext.Notifications.Add(dataModel);

            return Task.FromResult(notification);
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static IQueryable<NotificationDataModel> ApplySearch(
            IQueryable<NotificationDataModel> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();

            return query.Where(x =>
                x.Title.Contains(search) ||
                x.Content.Contains(search));
        }

        private static IQueryable<NotificationDataModel> ApplyFilters(
            IQueryable<NotificationDataModel> query,
            IReadOnlyList<FilterCriteria> filters)
        {
            foreach (var filter in filters)
            {
                var field = filter.Field.Trim();

                if (field.Equals("isRead", StringComparison.OrdinalIgnoreCase))
                {
                    if (bool.TryParse(filter.Value, out var isRead))
                        query = query.Where(x => x.IsRead == isRead);
                }
                else if (field.Equals("type", StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse<NotificationType>(filter.Value, true, out var type))
                        query = query.Where(x => x.Type == type);
                }
                else if (field.Equals("createdAt", StringComparison.OrdinalIgnoreCase))
                {
                    if (DateTime.TryParse(filter.Value, out var date))
                    {
                        var constant = date;
                        query = filter.Operator switch
                        {
                            FilterOperator.GreaterThan => query.Where(x => x.CreatedAt > constant),
                            FilterOperator.GreaterThanOrEqual => query.Where(x => x.CreatedAt >= constant),
                            FilterOperator.LessThan => query.Where(x => x.CreatedAt < constant),
                            FilterOperator.LessThanOrEqual => query.Where(x => x.CreatedAt <= constant),
                            _ => query.Where(x => x.CreatedAt == constant)
                        };
                    }
                }
            }

            return query;
        }

        private static IQueryable<NotificationDataModel> ApplySorting(
            IQueryable<NotificationDataModel> query,
            string? sortBy,
            SortDirection sortDirection)
        {
            var field = sortBy?.Trim();

            if (string.IsNullOrWhiteSpace(field))
                return query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id);

            var descending = sortDirection == SortDirection.Desc;

            return field.ToLowerInvariant() switch
            {
                "id" => descending ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
                "title" => descending ? query.OrderByDescending(x => x.Title) : query.OrderBy(x => x.Title),
                "type" => descending ? query.OrderByDescending(x => x.Type) : query.OrderBy(x => x.Type),
                "isread" => descending ? query.OrderByDescending(x => x.IsRead) : query.OrderBy(x => x.IsRead),
                "createdat" => descending
                    ? query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                    : query.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
                _ => query.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
            };
        }

        private static Notification MapToDomain(NotificationDataModel dataModel)
        {
            return new Notification(
                dataModel.Id,
                dataModel.UserId,
                dataModel.Title,
                dataModel.Content,
                dataModel.Type,
                dataModel.UrlLink,
                dataModel.IsRead,
                dataModel.CreatedAt);
        }
    }
}
