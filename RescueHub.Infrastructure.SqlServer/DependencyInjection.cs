using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace AgriTrace.Infrastructure.Sqlserver
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
