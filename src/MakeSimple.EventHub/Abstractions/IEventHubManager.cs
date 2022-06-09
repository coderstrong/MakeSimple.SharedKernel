using System;
using System.Collections.Generic;

namespace MakeSimple.EventHub.Abstractions
{
    public interface IEventHubManager
    {
        /// <summary>
        /// Subscribe handler of an event
        /// </summary>
        /// <typeparam name="E">Event type</typeparam>
        /// <typeparam name="EH">Event handler type</typeparam>
        void Subscribe<E, EH>() where E : IEvent where EH : IEventHandler<E>;

        /// <summary>
        /// Unsubscribe handler of an event
        /// </summary>
        /// <typeparam name="E">Event type</typeparam>
        /// <typeparam name="EH">Event handler type</typeparam>
        void Unsubscribe<E, EH>() where E : IEvent where EH : IEventHandler<E>;

        /// <summary>
        /// Unsubscribe all handler of an event
        /// </summary>
        /// <typeparam name="E">Event type</typeparam>
        void Unsubscribe<E>() where E : IEvent;

        /// <summary>
        /// Check have any subscribe of an event
        /// </summary>
        /// <typeparam name="E">Event type</typeparam>
        /// <returns></returns>
        bool HasSubscribe<E>() where E : IEvent;

        /// <summary>
        /// Check have any subscribe of an event
        /// </summary>
        /// <typeparam name="eventName">Event type</typeparam>
        /// <returns></returns>
        bool HasSubscribe(string eventName);

        /// <summary>
        /// Get list handlers of an event
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        IEnumerable<Type> GetHandlers<E>() where E : IEvent;

        /// <summary>
        /// Get list handlers of an event name
        /// </summary>
        /// <typeparam name="eventName"></typeparam>
        /// <returns></returns>
        IEnumerable<Type> GetHandlers(string eventName);

        public Type GetEventByName(string eventName);

        bool IsEmpty { get; }

        /// <summary>
        /// Empty events
        /// </summary>
        void Clear();
    }
}