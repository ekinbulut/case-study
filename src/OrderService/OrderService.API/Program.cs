using Common.Extensions;
using OrderService.Application.Handlers;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using Scalar.AspNetCore;

namespace OrderService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCommonServices(builder.Configuration);
        
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly));
        builder.Services.AddEntityFramework<OrderDbContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddTransient<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUnitOfWork<OrderDbContext>, UnitOfWork<OrderDbContext>>();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseErrorHandlingMiddleware();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}