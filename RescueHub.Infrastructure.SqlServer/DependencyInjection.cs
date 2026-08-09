using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Services;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Infrastructure.SqlServer.Repositories;

namespace RescueHub.Infrastructure.SqlServer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Đăng ký DbContext và kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.UseNetTopologySuite();
                    });
            });

            // Khi cần IUserRepository -> sử dụng UserRepository
            services.AddScoped<IUserRepository, UserRepository>();

            // Khi cần IUserService -> sử dụng UserService
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}