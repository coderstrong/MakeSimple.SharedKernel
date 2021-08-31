using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Infrastructure.Repository;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Services;

    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register db context,
        /// Library Sieve,
        /// RepositoryGeneric
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <param name="contextLifetime"></param>
        /// <param name="optionsLifetime"></param>
        /// <returns></returns>
        public static void AddEfDbContext<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, ServiceLifetime contextLifetime = ServiceLifetime.Scoped, ServiceLifetime optionsLifetime = ServiceLifetime.Scoped)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(optionsAction, contextLifetime, optionsLifetime);
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepositoryGeneric<,>));
            services.AddScoped<SieveProcessor>();
        }

        /// <summary>
        /// Register db context,
        /// Library Sieve,
        /// RepositoryGeneric
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="optionsAction"></param>
        /// <param name="poolSize"></param>
        public static void AddEfDbContextPool<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null, int poolSize = 128)
            where TContext : DbContext
        {
            services.AddDbContextPool<TContext>(optionsAction, poolSize);
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepositoryGeneric<,>));
            services.AddScoped<SieveProcessor>();
        }

        public static void AddHeaderForwardCore(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        public static void AddApiProfilerCore(this IServiceCollection services)
        {
            services.AddMiniProfiler(options =>
                options.RouteBasePath = "/profiler"
            ).AddEntityFramework();
        }
    }
}