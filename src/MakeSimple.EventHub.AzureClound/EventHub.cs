using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using MakeSimple.EventHub.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.AzureClound
{
    public class EventHub : IEventHub
    {
        private readonly IEventHubManager _eventBusManager;
        private readonly EventHubSettingOption _eventHubSetting;
        private readonly IEventHubPersistentConnection _persistentConnection;
        private readonly ILogger<EventHub> _logger;
        public EventHub(IEventHubPersistentConnection persistentConnection
            , IEventHubManager eventBusManager
            , IOptions<EventHubSettingOption> option
            , ILogger<EventHub> logger)
        {
            _eventHubSetting = option.Value;
            _eventBusManager = eventBusManager;
            _persistentConnection = persistentConnection;
            _logger = logger;
        }

        public async Task<string> PublishAsync<E>(E message) where E : IEvent
        {
            _logger.LogDebug($"Start PublishAsync with data {message.EventId}");

            SettingAttribute eventSettingAttr = typeof(E).GetCustomAttribute<SettingAttribute>();

            // Create a producer client that you can use to send events to an event hub
            var producerClient = new EventHubProducerClient(_eventHubSetting.ConnectionString, eventSettingAttr.EventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)))))
            {
                // if it is too large for the batch
                throw new FormatException($"Event {message.EventId} is too large for the batch and cannot be sent.");
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                return await Task.FromResult(string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"End PublishAsync with data {message.EventId}");
                throw;
            }
            finally
            {
                _logger.LogDebug($"End PublishAsync with data {message.EventId}");
                await producerClient.DisposeAsync();
            }
        }

        public Task StartListening()
        {

            if (_eventHubSetting.IsSubscription)
            {
                _persistentConnection.PersistentConnectAsync();
            }
            return Task.CompletedTask;
        }

        public Task SubscribeAsync<E, EH>(CancellationToken cancellationToken)
            where E : IEvent
            where EH : IEventHandler<E>
        {
            if (!_eventBusManager.HasSubscribe<E>())
            {
                _eventBusManager.Subscribe<E, EH>();
            }
            _logger.LogDebug($"Subscribed {typeof(E).Name}");

            return Task.CompletedTask;
        }

        public void Unsubscribe<E, EH>()
            where E : IEvent
            where EH : IEventHandler<E>
        {
            _eventBusManager.Unsubscribe<E, EH>();
            _logger.LogDebug($"Unsubscribed {typeof(E).Name}");
        }

        public void UnsubscribeAll<E>() where E : IEvent
        {
            _eventBusManager.Unsubscribe<E>();
            _logger.LogDebug($"UnsubscribeAll");
        }
    }
}
