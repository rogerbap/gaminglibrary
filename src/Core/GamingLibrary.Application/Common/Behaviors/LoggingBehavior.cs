// src/Core/GamingLibrary.Application/Common/Behaviors/LoggingBehavior.cs
// Purpose: MediatR pipeline behavior for logging all requests/responses
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GamingLibrary.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that logs all command and query executions.
    /// Provides performance monitoring and audit trail for all operations.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Handling {RequestName}: {@Request}", requestName, request);

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation("Completed {RequestName} in {ElapsedMilliseconds}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Failed {RequestName} after {ElapsedMilliseconds}ms", 
                    requestName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
