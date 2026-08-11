using Microsoft.EntityFrameworkCore; // Nhớ thêm namespace này nếu dùng UseSqlServer
using RescueHub.Domain.Interfaces;
using RescueHub.Infrastructure.SqlServer.Persistence;
using RescueHub.Domain.Services;
using RescueHub.Infrastructure.SqlServer.Repositories;
using RescueHub.Infrastructure.SqlServer.Services;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. ĐĂNG KÝ CÁC DỊCH VỤ (SERVICES)
// ==========================================

// Cấu hình CORS cho phép React gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Cổng chạy React của bạn
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký DbContext (SQL Server) và AuthService của bạn
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));


// Đăng ký AuthService để sử dụng trong Controller
builder.Services.AddScoped<IAuthService, AuthService>();

// Đăng ký AuthRepository để sử dụng trong AuthService
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// Đăng ký Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// ==========================================
// 2. CẤU HÌNH HTTP REQUEST PIPELINE
// ==========================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// QUAN TRỌNG: UseCors phải đặt trước UseAuthorization và MapControllers
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();