namespace RescueHub.Domain.Common
{
    /// Chứa các thuộc tính dùng chung: Id, CreatedAt, UpdatedAt.
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime? UpdatedAt { get; protected set; }


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
            DateTime? updatedAt)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }


        // Đánh dấu thời điểm Entity được cập nhật
        protected void MarkUpdated()
        {
            UpdatedAt = DateTime.Now;
        }

    }
}
