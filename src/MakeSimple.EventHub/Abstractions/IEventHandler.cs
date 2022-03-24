using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.Abstractions
{
    public interface IEventHandler
    {

    }
    public interface IEventHandler<E> : IEventHandler
    {
        public Task<bool> HandleAsync(E @event, CancellationToken cancellationToken);
    }
}
