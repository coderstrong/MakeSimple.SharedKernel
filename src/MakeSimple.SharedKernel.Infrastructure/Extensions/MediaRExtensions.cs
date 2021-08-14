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
    using FluentValidation.Results;
    using MakeSimple.SharedKernel.Extensions;
    using MakeSimple.SharedKernel.Infrastructure.DTO;
    using MakeSimple.SharedKernel.Infrastructure.Exceptions;
    using System.Net;
    using System.Reflection;

    public static class MediaRExtensions
    {
        public static void AddMediaRModule(this IServiceCollection services, MediaROptions options = null)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                options = new MediaROptions();
            }

            foreach (var pattern in options.EndWithPattern)
            {
                var assemblies = Assembly.GetEntryAssembly().GetReferencedAssemblies().Where(e => e.Name.EndsWith(pattern)).Select(e => Assembly.Load(e)).ToList();

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
        }
    }

    public class MediaROptions
    {
        public bool OnValidatorPipeline { get; set; }
        public bool OnLoggingPipeline { get; set; }
        public ICollection<string> EndWithPattern { get; set; }

        public MediaROptions()
        {
            OnValidatorPipeline = true;
            OnLoggingPipeline = false;
            EndWithPattern = new List<string> { ".Application" };
        }
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

            IEnumerable<ValidationFailure> failures = _validators
               .Select(v => v.Validate(request))
               .SelectMany(result => result.Errors)
               .Where(error => error != null);

            if (failures.Any())
            {
                _logger.LogWarning("Validation errors - {typeName} - Command: {@request} - Errors: {@failures}", typeName, request, failures);

                throw new ValidationException(new Response<bool>
                            (
                                HttpStatusCode.BadRequest,
                                new ErrorBase("ValidationError", string.Join(", ", failures.Select(err => err.ErrorMessage).ToArray()))
                            ));
            }

            return await next();
        }
    }
}