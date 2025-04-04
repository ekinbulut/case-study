using Common.Extensions;
using Common.Infrastructure;
using Scalar.AspNetCore;
using StockService.Application;
using StockService.Application.Handlers;
using StockService.Domain.Interfaces;
using StockService.Infrastructure.Data;
using StockService.Infrastructure.Repositories;
using ServiceCollectionExtensions = Common.Extensions.ServiceCollectionExtensions;

namespace StockService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCommonServices(builder.Configuration);
        
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStockCommandHandler).Assembly));
        builder.Services.AddEntityFramework<StockDbContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddTransient<IStockRepository, StockRepository>();
        builder.Services.AddScoped<IUnitOfWork<StockDbContext>, UnitOfWork<StockDbContext>>();

        builder.Services.AddHostedService<StockBackgroundService>();
        builder.Services.AddHostedService<StockUpdateBackgroundService>();
        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        using (var scope = app.Services.CreateScope())
        {
            var migrationService = scope.ServiceProvider.GetRequiredService<ServiceCollectionExtensions.IDatabaseMigrationService<StockDbContext>>();
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