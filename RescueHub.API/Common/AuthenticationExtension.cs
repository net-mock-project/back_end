using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RescueHub.API.Models;
using RescueHub.Application.Common.Settings;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace RescueHub.API.Common;

public static class AuthenticationExtension
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        var jwtOptions = configuration
            .GetSection("Jwt")
            .Get<JwtOptions>()!;

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.Key)),

                    RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    // Lấy JWT từ Cookie
                    OnMessageReceived = context =>
                    {
                        if (context.Request.Cookies.TryGetValue(
                            "rescuehub_token",
                            out var token))
                        {
                            context.Token = token;
                        }

                        return Task.CompletedTask;
                    },

                    // 401 - Chưa đăng nhập / Token không hợp lệ
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode =
                            StatusCodes.Status401Unauthorized;

                        var response = ApiResponse.Fail(
                            HttpStatusCode.Unauthorized,
                            new[]
                            {
                                "Bạn chưa đăng nhập hoặc token không hợp lệ."
                            });

                        await context.Response.WriteAsJsonAsync(
                            response);
                    },

                    // 403 - Đã đăng nhập nhưng không đủ quyền
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        var response = ApiResponse.Fail(
                            HttpStatusCode.Forbidden,
                            new[]
                            {
                                "Bạn không có quyền truy cập chức năng này."
                            });

                        await context.Response.WriteAsJsonAsync(
                            response);
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}