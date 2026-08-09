using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RescueHub.Infrastructure.SqlServer.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Thay chuỗi kết nối dưới đây bằng đúng ConnectionString trong file appsettings.json của dự án API bạn đang dùng
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=RescueHubDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                x => x.UseNetTopologySuite()
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}