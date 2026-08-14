using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Infrastructure.SqlServer.Repositories;
using RescueHub.Infrastructure.SqlServer.Security;
using RescueHub.Infrastructure.SqlServer.Services;

namespace RescueHub.Infrastructure.SqlServer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine(
                $"[DB] Connection: {connectionString}");

            // Đăng ký DbContext kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.UseNetTopologySuite()));

            // Đăng ký Auth Repository
            services.AddScoped<IAuthRepository, AuthRepository>();


            // Đăng ký User Repository
            services.AddScoped<IUserRepository, UserRepository>();

            // Đăng ký Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký Jwt
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ICacheService, DistributedCacheService>();

            // Đăng ký Email Service
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}