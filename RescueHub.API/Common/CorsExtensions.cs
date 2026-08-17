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
            // Đọc url từ config
            var frontendUrl = configuration["Cors:FrontendUrl"];

            services.AddCors(options =>
            {
                // Set policy chỉ chấp nhận api từ URL
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
