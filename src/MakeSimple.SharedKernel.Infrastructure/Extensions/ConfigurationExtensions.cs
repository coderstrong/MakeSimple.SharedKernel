using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace MakeSimple.SharedKernel.Infrastructure.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationRoot GetConfiguration(string basePath = null)
        {
            basePath ??= Directory.GetCurrentDirectory();

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{ Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
