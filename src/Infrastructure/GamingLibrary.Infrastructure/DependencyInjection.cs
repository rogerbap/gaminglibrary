// src/Infrastructure/GamingLibrary.Infrastructure/DependencyInjection.cs
// Purpose: Infrastructure layer dependency injection configuration
using GamingLibrary.Application.Common.Interfaces;
using GamingLibrary.Infrastructure.Persistence.Configurations;
using GamingLibrary.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace GamingLibrary.Infrastructure
{
    /// <summary>
    /// Extension methods for configuring infrastructure layer services
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds infrastructure layer services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Configure MongoDB
            services.Configure<MongoDbSettings>(
                configuration.GetSection("MongoDB"));

            services.AddSingleton<IMongoClient>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            services.AddScoped<IMongoDatabase>(serviceProvider =>
            {
                var client = serviceProvider.GetRequiredService<IMongoClient>();
                var settings = serviceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return client.GetDatabase(settings.DatabaseName);
            });

            // Configure MongoDB mappings
            ConfigureMongoDbMappings();

            // Register repositories
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IGameSessionRepository, GameSessionRepository>();

            return services;
        }

        private static void ConfigureMongoDbMappings()
        {
            // Configure entity mappings for MongoDB
            PlayerConfiguration.Configure();
            GameSessionConfiguration.Configure();
        }
    }

    /// <summary>
    /// MongoDB configuration settings
    /// </summary>
    public class MongoDbSettings
    {
        public const string SectionName = "MongoDB";
        
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int ConnectionTimeoutSeconds { get; set; } = 30;
        public bool EnableDetailedErrors { get; set; } = false;
    }
}
