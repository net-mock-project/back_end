using RescueHub.Domain.Common;

namespace RescueHub.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string Action { get; private set; } = null!;

        public string EntityName { get; private set; } = null!;

        public Guid EntityId { get; private set; }

        public string? OldValue { get; private set; }

        public string? NewValue { get; private set; }

        private AuditLog() { }

        public AuditLog(
            Guid id,
            Guid userId,
            string action,
            string entityName,
            Guid entityId,
            string? oldValue,
            string? newValue,
            DateTime createdAt)
            : base(id, createdAt, null, null)
        {
            UserId = userId;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            OldValue = oldValue;
            NewValue = newValue;
        }

        public AuditLog(
            Guid userId,
            string action,
            string entityName,
            Guid entityId,
            string? oldValue = null,
            string? newValue = null)
            : base()
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty.", nameof(action));

            if (string.IsNullOrWhiteSpace(entityName))
                throw new ArgumentException("Entity name cannot be empty.", nameof(entityName));

            if (entityId == Guid.Empty)
                throw new ArgumentException("Entity ID cannot be empty.", nameof(entityId));

            UserId = userId;
            Action = action;
            EntityName = entityName;
            EntityId = entityId;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
