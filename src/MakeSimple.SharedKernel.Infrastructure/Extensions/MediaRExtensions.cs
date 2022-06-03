﻿namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    using MakeSimple.SharedKernel.Extensions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

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
                    services.AddMediatR(assembly);
                }
            }

            if (options.OnLoggingPipeline)
                services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }
    }

    public class MediaROptions
    {
        public bool OnLoggingPipeline { get; set; }
        public ICollection<string> EndWithPattern { get; set; }

        public MediaROptions()
        {
            OnLoggingPipeline = false;
            EndWithPattern = new List<string> { ".Application" };
        }
    }

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
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
}