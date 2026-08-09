using Microsoft.EntityFrameworkCore;
using RescueHub.Domain.Entities;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        // Nhận cấu hình kết nối Database từ Dependency Injection
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Đại diện cho bảng User trong Database
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tự động đọc các file Entity Configuration
            // trong project Infrastructure
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
    }
}