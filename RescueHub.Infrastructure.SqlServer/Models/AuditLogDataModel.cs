namespace RescueHub.Infrastructure.SqlServer.Models
{
    public class AuditLogDataModel
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Action { get; set; } = null!;

        public string EntityName { get; set; } = null!;

        public Guid EntityId { get; set; }

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedAt { get; set; }

        public UserDataModel User { get; set; } = null!;
    }
}
