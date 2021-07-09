using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    using MakeSimple.SharedKernel.Extensions;
    using MakeSimple.SharedKernel.Infrastructure.DTO;
    using MakeSimple.SharedKernel.Infrastructure.Exceptions;
    using System.Net;

    public static class MediaRExtensions
    {
        public static IServiceCollection AddMediaRModule(this IServiceCollection services, MediaROptions options)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach (var pattern in options.PatternModules)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.GetName().Name.Contains(pattern)).ToList();

                foreach (var assembly in assemblies)
                {
                    AssemblyScanner.FindValidatorsInAssembly(assembly)
                    .ForEach(result =>
                    {
                        services.AddTransient(result.InterfaceType, result.ValidatorType);
                    });
                    services.AddMediatR(assembly);
                }
            }

            if (options.OnLoggingPipeline)
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            if (options.OnValidatorPipeline)
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            return services;
        }
    }

    public class MediaROptions
    {
        public bool OnValidatorPipeline { get; set; } = true;
        public bool OnLoggingPipeline { get; set; } = false;
        public ICollection<string> PatternModules { get; set; } = new List<string> { ".Application" };
    }

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _logger.LogInformation("Handling command {CommandName} ({@Command})", request.GetGenericTypeName(), request);
            var response = await next();
            _logger.LogInformation("Command {CommandName} handled - response: {@Response}", request.GetGenericTypeName(), response);

            return response;
        }
    }

    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validator, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validators = validator;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogDebug("Validating command {CommandType}", typeName);

            var failures = _validators
               .Select(v => v.Validate(request))
               .SelectMany(result => result.Errors)
               .Where(error => error != null);

            if (failures.Any())
            {
                _logger.LogWarning("Validation errors - {typeName} - Command: {@request} - Errors: {@failures}", typeName, request, failures);

                throw new ValidationException(new Response<bool>
                            (
                                HttpStatusCode.BadRequest,
                                new ErrorBase()
                                {
                                    Code = "ValidationError",
                                    ErrorMessage = string.Join(", ", failures.Select(err => $"{err.PropertyName}: {err.ErrorMessage}").ToArray())
                                }
                            ));
            }

            return await next();
        }
    }
}