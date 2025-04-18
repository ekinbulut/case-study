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
        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            
            var rabbitMqSection = configuration.GetSection("RabbitMQ");
            var hostname = rabbitMqSection["Hostname"];
            var username = rabbitMqSection["Username"];
            var password = rabbitMqSection["Password"];
            
            
            services.AddSingleton<EventBus>(sp =>
            {
                // Create EventBus synchronously by waiting for the async creation
                var eventbus = EventBus.CreateAsync(hostname, username, password).GetAwaiter()
                    .GetResult();
                
                eventbus.DeclareExchangeAsync().GetAwaiter().GetResult();
                
                return eventbus;
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

        /// <summary>
        /// Registers Entity Framework Core with PostgreSQL support.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
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

            services.AddSingleton<IDatabaseMigrationService<TContext>, DatabaseMigrationService<TContext>>();
            
            return services;
        }

        // Extension method for easy middleware registration

        public static IApplicationBuilder UseErrorHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingMiddleware>();
        }
        
        /// <summary>
        /// Registers the request logging middleware.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
        
        
        public interface IDatabaseMigrationService<TContext> where TContext : DbContext
        {
            void ApplyMigrations();
        }

        public class DatabaseMigrationService<TContext> : IDatabaseMigrationService<TContext> where TContext : DbContext
        {
            private readonly IServiceProvider _serviceProvider;

            public DatabaseMigrationService(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public void ApplyMigrations()
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                context.Database.Migrate();
            }
        }
    }
}