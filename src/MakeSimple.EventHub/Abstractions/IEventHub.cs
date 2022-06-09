using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.Abstractions
{
    public interface IEventHub
    {
        public Task<string> PublishAsync<E>(E message) where E : IEvent;

        public Task SubscribeAsync<E, EH>(CancellationToken cancellationToken) where E : IEvent where EH : IEventHandler<E>;

        public void Unsubscribe<E, EH>() where E : IEvent where EH : IEventHandler<E>;

        public void UnsubscribeAll<E>() where E : IEvent;

        public Task StartListening();
    }
}