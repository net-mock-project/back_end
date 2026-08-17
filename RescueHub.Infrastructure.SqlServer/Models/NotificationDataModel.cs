using RescueHub.Domain.Common.Enums;

namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class NotificationDataModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string? UrlLink { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserDataModel User { get; set; } = null!;
    }
}
