using MakeSimple.SharedKernel.Infrastructure.DTO;
using MakeSimple.SharedKernel.Infrastructure.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapperCore(this IServiceCollection services)
        {
            // To do
            return services;
        }

        public static IServiceCollection AddMediatRCore(this IServiceCollection services,
            IEnumerable<Assembly> registeredAssemblies)
        {
            services.AddMediatR(registeredAssemblies.ToArray());
            return services;
        }

        public static IServiceCollection AddRestClientCore(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddSingleton<IUrlHelper>(fac =>
                new UrlHelper(fac.GetService<IActionContextAccessor>().ActionContext));

            return services;
        }

        public static IServiceCollection AddCacheCore(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddResponseCaching();

            return services;
        }

        public static IServiceCollection AddApiVersionCore(this IServiceCollection services, IConfiguration config)
        {
            services.AddRouting(o => o.LowercaseUrls = true);

            services
                .AddMvcCore().AddJsonOptions(e => e.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
                .AddDataAnnotations();

            services
                .AddApiVersioning(o =>
                {
                    o.ReportApiVersions = true;
                    o.AssumeDefaultVersionWhenUnspecified = true;
                    o.DefaultApiVersion = ParseApiVersion(config.GetValue<string>("API_VERSION"));
                });

            return services;
        }

        public static IServiceCollection AddAuthNCore(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // To do

            return services;
        }

        public static IServiceCollection AddCorsCore(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllRequestPolicy",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .SetIsOriginAllowedToAllowWildcardSubdomains());
            });

            return services;
        }

        public static IServiceCollection AddHeaderForwardCore(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            return services;
        }

        public static IServiceCollection AddApiProfilerCore(this IServiceCollection services)
        {
            services.AddMiniProfiler(options =>
                options.RouteBasePath = "/profiler"
            ).AddEntityFramework();

            return services;
        }

        private static ApiVersion ParseApiVersion(string serviceVersion)
        {
            if (string.IsNullOrEmpty(serviceVersion)) throw new TryGetKeyNotFoundException(new Response<bool>
            (
                System.Net.HttpStatusCode.InternalServerError,
                new ErrorBase() { ErrorMessage = "[CS] ServiceVersion is null or empty.", Code = "ConfigNull" }
            ));

            const string pattern = @"(.)|(-)";
            var results = Regex.Split(serviceVersion, pattern)
                .Where(x => x != string.Empty && x != "." && x != "-")
                .ToArray();

            if (results == null || results.Length < 2) throw new TryGetKeyNotFoundException(new Response<bool>
            (
                System.Net.HttpStatusCode.InternalServerError,
                new ErrorBase() { ErrorMessage = "[CS] Could not parse ServiceVersion.", Code = "CouldNotParse" }
            ));

            if (results.Length > 2)
                return new ApiVersion(
                    Convert.ToInt32(results[0]),
                    Convert.ToInt32(results[1]),
                    results[2]);

            return new ApiVersion(
                Convert.ToInt32(results[0]),
                Convert.ToInt32(results[1]));
        }
    }
}
