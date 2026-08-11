namespace RescueHub.Domain.Common
{
    /// <summary>
    /// Lớp cơ sở (Base) cho mọi Entity trong tầng Domain.
    /// Gom các thuộc tính dùng chung: khóa chính (Id) và mốc thời gian (audit).
    /// Các Entity khác chỉ cần kế thừa lớp này để có sẵn Id, CreatedAt, UpdatedAt.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// Khóa chính (Primary Key) của Entity, kiểu dữ liệu là Guid.
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Mốc thời gian tạo (CreatedAt) của Entity, kiểu dữ liệu là DateTime.
        /// </summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>
        /// Mốc thời gian cập nhật (UpdatedAt) của Entity, kiểu dữ liệu là DateTime?.
        /// Không bắt buộc vì có thể Entity không được cập nhật sau khi tạo. 
        /// </summary>
        public DateTime? UpdatedAt { get; protected set; }

        /// <summary>
        /// Hàm khởi tạo mặc định của lớp BaseEntity.
        /// Tự động sinh Id mới và đặt CreatedAt là thời gian hiện tại (UTC).
        /// </summary>
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Hàm khởi tạo có tham số của lớp BaseEntity.
        /// Chỉ được sử dụng khi càn dựng lại Entity từ database, cần giữ nguyên các giá trị gốc.
        /// </summary>
        /// <param name="id">Id của Entity gốc của bản ghi trong database</param>
        /// <param name="createdAt">Mốc thời gian tạo gốc của bản ghi trong database</param>
        /// <param name="updatedAt">Mốc thời gian cập nhật (nếu có) của bản ghi trong database</param>
        protected BaseEntity(
            Guid id,
            DateTime createdAt,
            DateTime? updatedAt)
        {
            Id = id;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        /// <summary>
        /// Hàm đánh dấu Entity đã được cập nhật, tự động đặt UpdatedAt là thời gian hiện tại (UTC).
        /// Được dùng trong các phương thức nghiệp vụ khi thay đổi dữ liệu của Entity, để ghi nhận mốc thời gian cập nhật.
        /// </summary>
        protected void MarkUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
