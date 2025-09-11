using ColegiosBackend.Configuration;
using ColegiosBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuración de Entity Framework
builder.Services.AddDbContext<ColegiosDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddAllApplicationServices(builder.Configuration)
        .AddDevelopmentServices()
        .ValidateServiceRegistration();
}
else
{
    builder.Services
        .AddAllApplicationServices(builder.Configuration)
        .AddProductionServices();
}
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
