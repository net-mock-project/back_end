using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Infrastructure.SqlServer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RescueHub.Infrastructure.SqlServer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureSqlServer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            // Đăng ký DbContext kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.UseNetTopologySuite()));

            // Đăng ký User Repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Đăng ký Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}