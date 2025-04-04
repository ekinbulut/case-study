using Common.Extensions;
using Common.Infrastructure;
using NotificationService.Application;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;
using Scalar.AspNetCore;

namespace NotificationService.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddCommonServices(builder.Configuration);
        
        //builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateStockCommandHandler).Assembly));
        builder.Services.AddEntityFramework<NotificationDbContext>(builder.Configuration);

        builder.Services.AddControllers();

        builder.Services.AddTransient<INotificationRepository, NotificationRepository>();
        builder.Services.AddScoped<IUnitOfWork<NotificationDbContext>, UnitOfWork<NotificationDbContext>>();

        builder.Services.AddHostedService<NotificationBackgroundService>();
        
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
        app.UseRequestLogging();

        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}