using MakeSimple.EventHub.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MakeSimple.EventHub
{
    public class InMemoryEventHubManager : IEventHubManager
    {
        private static readonly object _lock = new object();
        private readonly ConcurrentDictionary<string, List<Type>> _eventNameAndHandlerMapping;
        private readonly ConcurrentDictionary<string, Type> _eventNameMappingEvent;

        public InMemoryEventHubManager()
        {
            _eventNameAndHandlerMapping = new ConcurrentDictionary<string, List<Type>>();
            _eventNameMappingEvent = new ConcurrentDictionary<string, Type>();
        }

        public bool IsEmpty => !_eventNameAndHandlerMapping.Keys.Any();

        public void Clear() => _eventNameAndHandlerMapping.Clear();

        public void Subscribe<E, EH>()
            where E : IEvent
            where EH : IEventHandler<E>
        {
            string eventName = typeof(E).Name;
            lock (_lock)
            {
                if (!HasSubscribe<E>())
                {
                    var handlers = new List<Type>() { typeof(EH) };
                    _eventNameAndHandlerMapping.TryAdd(eventName, handlers);
                    _eventNameMappingEvent.TryAdd(eventName, typeof(E));
                }
                else if (_eventNameAndHandlerMapping[eventName].All(h => h != typeof(EH)))
                {
                    _eventNameAndHandlerMapping[eventName].Add(typeof(EH));
                }
            }
        }

        public void Unsubscribe<E, EH>()
            where E : IEvent
            where EH : IEventHandler<E>
        {
            string eventName = typeof(E).Name;
            lock (_lock)
            {
                _eventNameAndHandlerMapping[eventName].Remove(typeof(EH));
                if (!_eventNameAndHandlerMapping[eventName].Any())
                {
                    _eventNameAndHandlerMapping.TryRemove(eventName, out _);
                }

                if (_eventNameMappingEvent.ContainsKey(eventName))
                {
                    _eventNameMappingEvent.TryRemove(eventName, out _);
                }
            }
        }

        public void Unsubscribe<E>() where E : IEvent
        {
            string eventName = typeof(E).Name;
            lock (_lock)
            {
                _eventNameAndHandlerMapping.TryRemove(eventName, out _);
            }
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _eventNameMappingEvent.FirstOrDefault(e => e.Key == eventName).Value;
        }

        public IEnumerable<Type> GetHandlers(string eventName)
        {
            return HasSubscribe(eventName) ? _eventNameAndHandlerMapping[eventName] : new List<Type>();
        }

        public IEnumerable<Type> GetHandlers<E>() where E : IEvent
        {
            return HasSubscribe<E>() ? _eventNameAndHandlerMapping[typeof(E).Name] : new List<Type>();
        }

        public bool HasSubscribe<E>() where E : IEvent
        {
            return _eventNameAndHandlerMapping.ContainsKey(typeof(E).Name);
        }

        public bool HasSubscribe(string eventName)
        {
            return _eventNameAndHandlerMapping.ContainsKey(eventName);
        }

        public Type GetEventByName(string eventName)
        {
            return _eventNameMappingEvent.GetValueOrDefault(eventName);
        }
    }
}