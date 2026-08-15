using RescueHub.API;
using RescueHub.API.Common;
using RescueHub.Application;
using RescueHub.Infrastructure.SqlServer;

var builder = WebApplication.CreateBuilder(args);


// Presentation / API
builder.Services.AddControllers();
builder.Services.AddPresentation();

// Application
builder.Services.AddApplication();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);


// Cache & CORS
builder.Services.AddDistributedMemoryCache();
builder.Services.AddCorsPolicy(builder.Configuration);

// JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Build Application
var app = builder.Build();


// Exception Handling
app.UseExceptionHandler();


// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// HTTP Pipeline

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

