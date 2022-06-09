using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using MakeSimple.EventHub.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.GoogleClound
{
    /// <summary>
    ///
    /// </summary>
    /// <exception cref="CustomAttributeFormatException">All EventHubSettingAttribute setting in event must have value</exception>
    public class EventHubBootstrap : IEventHubPersistentConnection
    {
        public HashSet<SettingAttribute> PubSubs { get; private set; }
        private readonly List<SubscriberClient> _subs;
        private readonly ILogger<EventHub> _logger;
        private readonly IServiceScopeFactory _serviceScope;
        private readonly IEventHubManager _eventBusManager;
        private readonly EventHubSettingOption _settingOption;

        public EventHubBootstrap(IOptions<EventHubSettingOption> option
            , ILogger<EventHub> logger
            , IServiceScopeFactory serviceScope
            , IEventHubManager eventBusManager)
        {
            _subs = new List<SubscriberClient>();
            _logger = logger;
            _serviceScope = serviceScope;
            _eventBusManager = eventBusManager;
            _settingOption = option.Value;

            PubSubs = new HashSet<SettingAttribute>();
        }

        public bool IsConnected => true;

        private async Task<bool> HandleEvent(PubsubMessage message, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start proccess receive a event has routing key with data {message}", message);
            try
            {
                var eventName = message.Attributes[Constant.RoutingKey];
                var eventType = _eventBusManager.GetEventByName(eventName);

                if (eventType == null)
                {
                    _logger.LogWarning($"Receive invaild event {eventName}. We don't have any handler for it");
                    await Task.Delay(5000);
                    return false;
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
                                bool result = await ((Task<bool>)concreteType.GetMethod("HandleAsync").Invoke(handler, new object[] { JsonSerializer.Deserialize(message.Data.ToStringUtf8(), eventType), cancellationToken }));
                                _logger.LogDebug("End proccess receive a event with data {result}", result);
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception Message: {ex.Message}");
            }
            return false;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken)
        {
            foreach (var item in _subs)
            {
                await item.StopAsync(cancellationToken);
            }
        }

        public void PersistentConnectAsync()
        {
            // Try create topic and subcription

            PublisherServiceApiClient publisherService;
            SubscriberServiceApiClient subscriberService;
            if (_settingOption.IsSimulatorMode)
            {
                publisherService = new PublisherServiceApiClientBuilder
                {
                    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                }.Build();
                subscriberService = new SubscriberServiceApiClientBuilder
                {
                    EmulatorDetection = EmulatorDetection.EmulatorOrProduction
                }.Build();
            }
            else
            {
                publisherService = PublisherServiceApiClient.Create();
                subscriberService = SubscriberServiceApiClient.Create();
            }

            var typeEvents = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(e => e.GetInterfaces()
                .Any(x => x == typeof(IEvent)))
                .Where(e => e.IsDefined(typeof(SettingAttribute)));

            foreach (var type in typeEvents)
            {
                SettingAttribute eventSettingAttr = type.GetCustomAttribute<SettingAttribute>();
                if (string.IsNullOrEmpty(eventSettingAttr.ProjectId) && string.IsNullOrEmpty(_settingOption.ProjectId))
                {
                    throw new CustomAttributeFormatException("ProjectId must had on config or attribue");
                }

                if (string.IsNullOrEmpty(eventSettingAttr.TopicId))
                {
                    throw new CustomAttributeFormatException("TopicId must had");
                }

                if (string.IsNullOrEmpty(eventSettingAttr.ProjectId))
                {
                    eventSettingAttr.ProjectId = _settingOption.ProjectId;
                }

                if (_eventBusManager.HasSubscribe(type.Name) && _settingOption.IsSubscription && !PubSubs.Any(e => e.ProjectId == eventSettingAttr.ProjectId
                && e.TopicId == eventSettingAttr.TopicId && e.SubscriptionId == eventSettingAttr.SubscriptionId))
                {
                    PubSubs.Add(eventSettingAttr);
                }
                else if (_eventBusManager.HasSubscribe(type.Name) && !PubSubs.Any(e => e.ProjectId == eventSettingAttr.ProjectId && e.TopicId == eventSettingAttr.TopicId))
                {
                    PubSubs.Add(eventSettingAttr);
                }
            }

            foreach (var topic in PubSubs)
            {
                TopicName topicName = new TopicName(topic.ProjectId, string.Concat(topic.TopicId, _settingOption.Suffix));
                try
                {
                    publisherService.CreateTopic(topicName);
                }
                catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
                {
                    Console.WriteLine($"Topic {topicName} already exists.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Can't create topic {topicName}. Trace: {e.StackTrace}");
                    throw;
                }

                if (_settingOption.IsSubscription && !string.IsNullOrEmpty(topic.SubscriptionId))
                {
                    SubscriptionName subscriptionName = new SubscriptionName(topic.ProjectId, string.Concat(topic.SubscriptionId, _settingOption.Suffix));
                    try
                    {
                        subscriberService.CreateSubscription(subscriptionName, topicName, pushConfig: null, ackDeadlineSeconds: 30);
                    }
                    catch (RpcException e) when (e.Status.StatusCode == StatusCode.AlreadyExists)
                    {
                        Console.WriteLine($"Topic {topicName} already exists.");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Can't create subscription {subscriptionName}. Trace: {e.StackTrace}");
                        throw;
                    }
                }
            }

            if (!_settingOption.IsSubscription)
            {
                return;
            }

            foreach (var sub in PubSubs.Distinct(new ComparerSubscription()))
            {
                // Pull messages from the subscription using SubscriberClient.
                SubscriptionName subscriptionName = SubscriptionName.FromProjectSubscription(sub.ProjectId, string.Concat(sub.SubscriptionId, _settingOption.Suffix));
                SubscriberClient subscriber = SubscriberClient.Create(subscriptionName);

                _subs.Add(subscriber);

                // Start the subscriber listening for messages.
                subscriber.StartAsync(async (msg, cancellationToken) =>
                {
                    return (await HandleEvent(msg, cancellationToken)) ? SubscriberClient.Reply.Ack : SubscriberClient.Reply.Nack;
                });
            }
        }
    }
}