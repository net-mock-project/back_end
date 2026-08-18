using RescueHub.Domain.Common.Enums;

namespace RescueHub.API.Models.Notifications
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string? UrlLink { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
