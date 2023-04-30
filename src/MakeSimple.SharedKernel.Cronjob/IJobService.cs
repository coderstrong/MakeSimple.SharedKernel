using System.Threading;
using System.Threading.Tasks;

namespace Trading.Core.Infrastructure.Cronjob
{
    public interface IJobService
    {
        public Task RegisterAsync(CancellationToken cancellation);
        public Task RegisterAsync(string cronCode, string cronEx, CancellationToken cancellation);
        public Task RemoveAsync(CancellationToken cancellation);
        public Task RemoveAsync(string cronCode, CancellationToken cancellation);
        public Task ExecuteAsync(params string[] inputs);
    }
}
