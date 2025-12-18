using AirportManagement.Application.Configurations;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Middleware;
using AirportManagement.Application.Services;
using AirportManagement.Infrastructure.Mappers;
using AirportManagement.Infrastructure.Repositories;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IFlightService, FlightService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AirportManagementContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AirportManagementConnectionString")));

builder.Services.AddAutoMapper(options =>{},
typeof(EfToDomainMapper).Assembly,
typeof(MapperConfig).Assembly
);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
