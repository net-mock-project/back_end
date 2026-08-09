using RescueHub.API;
using RescueHub.Infrastructure.SqlServer;

var builder = WebApplication.CreateBuilder(args);

// Đăng ký Controller, Global Exception Handler,
// ApiResponseWrapperFilter...
builder.Services.AddPresentation();

// Đăng ký:
// ApplicationDbContext
// IUserRepository -> UserRepository
// IUserService -> UserService
builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Kích hoạt Global Exception Handler
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Nếu team đã cấu hình JWT Authentication
// thì UseAuthentication phải đứng trước UseAuthorization
// app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();