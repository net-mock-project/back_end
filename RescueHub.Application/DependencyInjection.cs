using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Application.Common.Behaviors;
using RescueHub.Domain.Interfaces.AuditLogs;
using RescueHub.Domain.Interfaces.Auth;
using RescueHub.Domain.Interfaces.Notifications;
using RescueHub.Domain.Interfaces.Users;
using RescueHub.Domain.Interfaces.Donations;
using RescueHub.Domain.Interfaces.Volunteers;
using RescueHub.Domain.Services;
using System.Reflection;

namespace RescueHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Đăng ký MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(
                    typeof(ValidationBehavior<,>));
            });
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Đăng ký Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            // Đăng ký Domain Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<IVolunteerService, VolunteerService>();

            return services;
        }
    }
}