using RescueHub.API;
using RescueHub.Application;
using RescueHub.Infrastructure.SqlServer;
using RescueHub.API.Common;
using RescueHub.Domain.Interfaces;
using RescueHub.Domain.Services; 
using RescueHub.Infrastructure.SqlServer.Repositories;
using RescueHub.Infrastructure.SqlServer.Services; 

var builder = WebApplication.CreateBuilder(args);

// Đăng ký các layer
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddCorsPolicy(builder.Configuration);

builder.Services.AddDistributedMemoryCache(); 

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Sửa từ 'services' thành 'builder.Services'
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Đăng ký EmailService (thay thế EmailService bằng tên class thực tế trong tầng Infrastructure của bạn đang implement IEmailService)
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Xử lý exception toàn cục
app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("FrontendPolicy");
app.UseAuthentication(); // Thêm UseAuthentication trước UseAuthorization nếu dùng JWT
app.UseAuthorization();
app.MapControllers();
app.Run();