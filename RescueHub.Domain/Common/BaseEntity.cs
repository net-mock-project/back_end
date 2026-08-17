namespace RescueHub.Domain.Common
{
    /// Chứa các thuộc tính dùng chung: Id, CreatedAt, UpdatedAt.
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }

        public DateTime? DeletedAt { get; protected set; }


        // Dùng khi tạo mới Entity
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
        }

        // Dùng khi dựng lại Entity từ database
        protected BaseEntity(
            Guid id,
            DateTime createdAt,
            DateTime? updatedAt,
            DateTime? deletedAt)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            DeletedAt = deletedAt;
        }


        // Đánh dấu thời điểm Entity được cập nhật
        protected void MarkUpdated()
        {
            UpdatedAt = DateTime.Now;
        }

        // Đánh dấu thời điểm Entity bị xóa
        protected void MarkDeleted()
        {
            DeletedAt = DateTime.Now;
        }
    }
}
