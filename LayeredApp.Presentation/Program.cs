using LayeredApp.Application.Services;
using LayeredApp.Domain;
using LayeredApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// ===========================================
// DEPENDENCY INJECTION CONFIGURATION
// ===========================================
// This is where we connect all the layers together
// Following the Dependency Inversion Principle (DIP)

// Register Infrastructure layer services
builder.Services.AddScoped<IProductRepository, InMemoryProductRepository>();

// Register Application layer services
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
