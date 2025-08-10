
// src/Core/GamingLibrary.Application/Common/Behaviors/PerformanceBehavior.cs
// Purpose: MediatR pipeline behavior for performance monitoring
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GamingLibrary.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that monitors performance and logs slow operations.
    /// Helps identify performance bottlenecks in the application.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private const int SlowRequestThresholdMs = 5000; // 5 seconds

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > SlowRequestThresholdMs)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("Slow Request: {RequestName} took {ElapsedMilliseconds}ms to execute. Request: {@Request}",
                    requestName, elapsedMilliseconds, request);
            }

            return response;
        }
    }
}