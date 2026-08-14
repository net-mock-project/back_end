using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Application.Services;
using RescueHub.Domain.Interfaces;
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
                cfg.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly()));

            // Đăng ký Mapster
            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            // Đăng ký Domain Service
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}