using RescueHub.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueHub.Infrastructure.SqlServer.Persistence;

namespace RescueHub.Infrastructure.SqlServer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureSqlServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    x => x.UseNetTopologySuite() // <-- Dòng này giúp EF Core hiểu và map kiểu Point sang geography của SQL Server
                ));

            return services;
        }
    }
}