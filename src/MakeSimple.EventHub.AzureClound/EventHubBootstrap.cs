using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs;
using MakeSimple.EventHub.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs.Processor;
using System.Text.Json;

namespace MakeSimple.EventHub.AzureClound
{
    public class EventHubBootstrap : IEventHubPersistentConnection
    {
        public HashSet<SettingAttribute> PubSubs { get; private set; }
        private readonly List<EventProcessorClient> _subs;
        private readonly ILogger<EventHub> _logger;
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IEventHubManager _eventBusManager;
        private readonly EventHubSettingOption _settingOption;
        public EventHubBootstrap(IOptions<EventHubSettingOption> option
            , ILogger<EventHub> logger
            , IServiceScopeFactory serviceScope
            , IEventHubManager eventBusManager)
        {
            _subs = new List<EventProcessorClient>();
            _logger = logger;
            _serviceScope = serviceScope;
            _eventBusManager = eventBusManager;
            _settingOption = option.Value;

            PubSubs = new HashSet<SettingAttribute>();
        }

        public bool IsConnected => true;

        public async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            _logger.LogDebug("Start proccess receive a event has routing key with data {message}", eventArgs.Data);

            var eventName = (string)eventArgs.Data.Properties[Constant.RoutingKey];
            var eventType = _eventBusManager.GetEventByName(eventName);

            if (eventType == null)
            {
                _logger.LogWarning($"Receive invaild event {eventName}. We don't have any handler for it");
                await Task.Delay(5000);
            }

            if (_eventBusManager.HasSubscribe(eventName))
            {
                var handlerTypes = _eventBusManager.GetHandlers(eventName);
                if (handlerTypes.Any())
                {
                    using var scope = _serviceScope.CreateScope();
                    if (typeof(IEventHandler).IsAssignableFrom(handlerTypes.First()))
                    {
                        foreach (var handlerType in handlerTypes)
                        {
                            var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                            if (handler == null) continue;
                            var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);
                            bool result = await ((Task<bool>)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { JsonSerializer.Deserialize(eventArgs.Data.EventBody.ToString(), eventType) }));
                            _logger.LogDebug("End proccess receive a event with data {result}", result);
                        }
                    }
                }
            }
        }

        public async Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {

            //
            _logger.LogError(eventArgs.Exception, $"Exception Message: {eventArgs.Exception.Message}");
            await Task.CompletedTask;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _subs)
            {
                await item.StopProcessingAsync(cancellationToken);
            }
        }

        public void PersistentConnectAsync()
        {
            // Try create topic and subcription
            if (!_settingOption.IsSubscription)
            {
                return;
            }

            var typeEvents = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(e => e.GetInterfaces()
                .Any(x => x == typeof(IEvent)))
                .Where(e => e.IsDefined(typeof(SettingAttribute)));

            foreach (var sub in PubSubs.Distinct(new ComparerSubscription()))
            {
                // Read from the default consumer group: $Default
                string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

                // Create a blob container client that the event processor will use 
                var storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

                // Create an event processor client to process events in the event hub
                var processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

                // Register handlers for processing events and handling errors
                processor.ProcessEventAsync += ProcessEventHandler;
                processor.ProcessErrorAsync += ProcessErrorHandler;

                _subs.Add(processor);
                // Start the processing
                processor.StartProcessingAsync();
            }
        }
    }
}
