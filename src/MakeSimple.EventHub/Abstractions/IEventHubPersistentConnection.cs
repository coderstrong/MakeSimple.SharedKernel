using System.Threading;
using System.Threading.Tasks;

namespace MakeSimple.EventHub.Abstractions
{
    public interface IEventHubPersistentConnection
    {
        public bool IsConnected { get; }

        public void PersistentConnectAsync();

        public Task DisconnectAsync(CancellationToken cancellationToken);
    }
}
