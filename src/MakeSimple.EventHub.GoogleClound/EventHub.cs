using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using MakeSimple.EventHub.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.GoogleClound
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
            _logger.LogDebug("Start PublishAsync with data {message}", message);

            SettingAttribute eventSettingAttr = typeof(E).GetCustomAttribute<SettingAttribute>();

            if (string.IsNullOrEmpty(eventSettingAttr.ProjectId))
            {
                eventSettingAttr.ProjectId = _eventHubSetting.ProjectId;
            }

            // Pull messages from the subscription using SubscriberClient.
            TopicName topicName = TopicName.FromProjectTopic(eventSettingAttr.ProjectId, string.Concat(eventSettingAttr.TopicId, _eventHubSetting.Suffix));
            PublisherClient publisher = await PublisherClient.CreateAsync(topicName);

            PubsubMessage pubsubMessage = new();
            pubsubMessage.PublishTime = Timestamp.FromDateTime(DateTime.UtcNow);
            // Add routing key for filter subscription
            pubsubMessage.Attributes.Add(Constant.RoutingKey, typeof(E).Name);
            pubsubMessage.Data = ByteString.CopyFromUtf8(JsonSerializer.Serialize(message));

            _logger.LogDebug("End PublishAsync with data response {pubsubMessage}", pubsubMessage);

            return await publisher.PublishAsync(pubsubMessage);
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