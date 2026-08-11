namespace RescueHub.API.Common
{
    /// <summary>
    /// Triển khai CORS nhận cổng local FE
    /// </summary>
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var frontendUrl = configuration["Cors:FrontendUrl"];

            services.AddCors(options =>
            {
                options.AddPolicy("FrontendPolicy", policy =>
                {
                    policy
                        .WithOrigins(frontendUrl!)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
