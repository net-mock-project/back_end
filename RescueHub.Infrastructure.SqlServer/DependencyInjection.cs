using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Application.Common.Interfaces;
using RescueHub.Application.Common.Settings;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Auth;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Interfaces.Roles;
using RescueHub.Domain.Interfaces.Volunteers;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Infrastructure.SqlServer.Repositories;
using RescueHub.Infrastructure.SqlServer.Seeds;
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

            // Đăng ký kết nối Cloudinary
            services.Configure<CloudinaryOptions>(
                configuration.GetSection(
                    CloudinaryOptions.SectionName));

            // Đăng ký implementation Cloudinary cho dịch vụ lưu trữ file
            services.AddScoped<
                IFileStorageService,
                CloudinaryFileStorageService>();

            // Đăng ký DbContext kết nối SQL Server
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlOptions => sqlOptions.UseNetTopologySuite()));

            services.AddScoped<DatabaseSeeder>();

            // Đăng ký Auth Repository
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IVolunteerRepository, VolunteerRepository>();

            // Đăng ký Donation Repository
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<RescueHub.Domain.Interfaces.ReliefRequests.IReliefRequestRepository, ReliefRequestRepository>();

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