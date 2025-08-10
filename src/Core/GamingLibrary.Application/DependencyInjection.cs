// src/Core/GamingLibrary.Application/DependencyInjection.cs
// Purpose: Application layer dependency injection configuration
using FluentValidation;
using GamingLibrary.Application.Common.Behaviors;
using GamingLibrary.Application.Common.Mappings;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GamingLibrary.Application
{
    /// <summary>
    /// Extension methods for configuring application layer services
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds application layer services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Add MediatR for CQRS pattern
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(assembly);
                
                // Add pipeline behaviors in order of execution
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            });

            // Add FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // Add AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PlayerMappingProfile>();
                cfg.AddProfile<GameSessionMappingProfile>();
            });

            return services;
        }
    }
}