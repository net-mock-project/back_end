using RescueHub.Domain.Common;
using RescueHub.Domain.Common.Enums;

namespace RescueHub.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string Title { get; private set; } = null!;

        public string Content { get; private set; } = null!;

        public NotificationType Type { get; private set; }

        public string? UrlLink { get; private set; }

        public bool IsRead { get; private set; }

        private Notification() { }

        // Dùng khi dựng lại Notification đã tồn tại từ database
        public Notification(
            Guid id,
            Guid userId,
            string title,
            string content,
            NotificationType type,
            string? urlLink,
            bool isRead,
            DateTime createdAt)
            : base(id, createdAt, null, null)
        {
            UserId = userId;
            Title = title;
            Content = content;
            Type = type;
            UrlLink = urlLink;
            IsRead = isRead;
        }

        // Dùng khi tạo mới Notification
        public Notification(
            Guid userId,
            string title,
            string content,
            NotificationType type,
            string? urlLink = null)
            : base()
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));

            UserId = userId;
            Title = title;
            Content = content;
            Type = type;
            UrlLink = urlLink;
            IsRead = false;
        }

        // Đánh dấu thông báo đã đọc
        public void MarkAsRead()
        {
            if (IsRead)
                return;

            IsRead = true;
        }
    }
}
