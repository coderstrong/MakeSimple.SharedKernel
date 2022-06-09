using MakeSimple.EventHub.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace MakeSimple.EventHub.GoogleClound
{
    public static class EventHubCollectionExtentions
    {
        public static void AddEventBusGoogleProvider(this IServiceCollection services, Action<EventHubSettingOption> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }
            services.AddOptions();
            services.Configure(setupAction);
            services.AutoLoadHandlers();
            services.AddSingleton<IEventHubManager, InMemoryEventHubManager>();
            services.AddSingleton<IEventHubPersistentConnection, EventHubBootstrap>();
            services.AddSingleton<IEventHub, EventHub>();
        }

        public static void AutoLoadHandlers(this IServiceCollection services)
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(e => e.GetInterfaces()
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEventHandler<>)));

            foreach (var item in types)
            {
                services.AddTransient(item);
            }
        }
    }
}