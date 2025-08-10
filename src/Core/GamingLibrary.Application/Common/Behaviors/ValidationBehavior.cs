// src/Core/GamingLibrary.Application/Common/Behaviors/ValidationBehavior.cs
// Purpose: MediatR pipeline behavior for automatic validation using FluentValidation
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GamingLibrary.Application.Common.Behaviors
{
    /// <summary>
    /// MediatR pipeline behavior that automatically validates all commands and queries
    /// using FluentValidation before they reach their handlers.
    /// </summary>
    /// <typeparam name="TRequest">The request type</typeparam>
    /// <typeparam name="TResponse">The response type</typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

        public ValidationBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var requestName = typeof(TRequest).Name;
            _logger.LogDebug("Validating {RequestName}", requestName);

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(result => !result.IsValid)
                .SelectMany(result => result.Errors)
                .ToArray();

            if (failures.Length > 0)
            {
                _logger.LogWarning("Validation failed for {RequestName}: {ValidationErrors}",
                    requestName, string.Join("; ", failures.Select(f => f.ErrorMessage)));

                throw new ValidationException($"Validation failed for {requestName}", failures);
            }

            _logger.LogDebug("Validation passed for {RequestName}", requestName);
            return await next();
        }
    }
}