using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
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
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Infrastructure.DTO;
    using MakeSimple.SharedKernel.Infrastructure.Exceptions;
    using MakeSimple.SharedKernel.Infrastructure.Repository;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Services;

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

        /// <summary>
        /// Register db context
        /// Library Sieve
        /// RepositoryGeneric
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="optionsLifetime"></param>
        /// <returns></returns>
        public static IServiceCollection AddEfDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(optionsAction, contextLifetime, optionsLifetime);
            services.AddScoped(typeof(IAuditRepository<,>), typeof(EfAuditRepositoryGeneric<,>));
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepositoryGeneric<,>));
            services.AddScoped<SieveProcessor>();

            return services;
        }

        public static IServiceCollection AddEfDbContextPool<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, int poolSize = 128)
            where TContext : DbContext
        {
            services.AddDbContextPool<TContext>(optionsAction, poolSize);
            services.AddScoped(typeof(IAuditRepository<,>), typeof(EfAuditRepositoryGeneric<,>));
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepositoryGeneric<,>));
            services.AddScoped<SieveProcessor>();

            return services;
        }

        public static IServiceCollection AddApiVersionCore(this IServiceCollection services, string ApiVersion = "1.0")
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
                    o.DefaultApiVersion = ParseApiVersion(ApiVersion);
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
                new ErrorBase("ConfigNull", "[CS] ServiceVersion is null or empty.")
            ));

            const string pattern = @"(.)|(-)";
            var results = Regex.Split(serviceVersion, pattern)
                .Where(x => x != string.Empty && x != "." && x != "-")
                .ToArray();

            if (results == null || results.Length < 2) throw new TryGetKeyNotFoundException(new Response<bool>
            (
                System.Net.HttpStatusCode.InternalServerError,
                new ErrorBase("CouldNotParse", "[CS] Could not parse ServiceVersion.")
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