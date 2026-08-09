using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RescueHub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services)
        {
            // Đăng ký các MediatR Handler trong Application project
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(
                    Assembly.GetExecutingAssembly());
            });

            return services;
        }
    }
}