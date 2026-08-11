using RescueHub.API;
using RescueHub.Application;
using RescueHub.Infrastructure.SqlServer;

using RescueHub.API.Common;


var builder = WebApplication.CreateBuilder(args);

// Đăng ký các layer
builder.Services.AddPresentation();
builder.Services.AddApplication();
builder.Services.AddInfrastructureSqlServer(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddCorsPolicy(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();