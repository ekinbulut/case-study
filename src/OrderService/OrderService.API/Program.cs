using Common.Extensions;
using Common.Infrastructure;
using OrderService.Application.Handlers;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using Scalar.AspNetCore;
using ServiceCollectionExtensions = Common.Extensions.ServiceCollectionExtensions;

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
        builder.Services.AddSwaggerGen();


        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var migrationService = scope.ServiceProvider.GetRequiredService<ServiceCollectionExtensions.IDatabaseMigrationService<OrderDbContext>>();
            migrationService.ApplyMigrations();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        app.MapScalarApiReference();
        app.UseSwagger();
        app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });

        
        app.UseErrorHandlingMiddleware();
        app.UseRequestLogging();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}