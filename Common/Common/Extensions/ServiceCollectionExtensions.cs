using Microsoft.Extensions.DependencyInjection;
using Common.Messaging;
using Common.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Common.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers common services used by all microservices, such as the EventBus.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rabbitMqHostname">The RabbitMQ host name.</param>
        /// <param name="rabbitMqUsername">The RabbitMQ username.</param>
        /// <param name="rabbitMqPassword">The RabbitMQ password.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddCommonServices(this IServiceCollection services, string rabbitMqHostname,
            string rabbitMqUsername, string rabbitMqPassword)
        {
            services.AddSingleton<EventBus>(sp =>
            {
                // Create EventBus synchronously by waiting for the async creation
                return EventBus.CreateAsync(rabbitMqHostname, rabbitMqUsername, rabbitMqPassword).GetAwaiter()
                    .GetResult();
            });

            return services;
        }

        /// <summary>
        /// Registers MediatR and its dependencies.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">Assemblies to scan for MediatR handlers.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddMediatrRServices<T>(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<T>());
            return services;
        }

        public static IServiceCollection AddEntityFramework<TContext>(this IServiceCollection services,
            IConfiguration configuration)
            where TContext : DbContext
        {
            // services.AddDbContext<TContext>(options =>
            //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")))
                
            services.AddDbContext<TContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    npgsqlOptions => 
                        npgsqlOptions.MigrationsAssembly(typeof(TContext).Assembly.FullName)));

            return services;
        }

        // Extension method for easy middleware registration

        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}