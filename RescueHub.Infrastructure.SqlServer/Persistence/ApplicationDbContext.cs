using Microsoft.EntityFrameworkCore;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    /// <summary>
    /// Context cơ sở dữ liệu chính của ứng dụng, quản lý các tập thực thể và cấu hình Entity Framework Core.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Khởi tạo một thể hiện mới của lớp <see cref="ApplicationDbContext"/> với các tùy chọn cấu hình.
        /// </summary>
        /// <param name="options">Các tùy chọn cấu hình cho <see cref="DbContext"/>.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Cấu hình mô hình dữ liệu và các mối quan hệ thực thể khi cơ sở dữ liệu đang được tạo.
        /// </summary>
        /// <param name="modelBuilder">Đối tượng được sử dụng để xây dựng mô hình cho context này.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
