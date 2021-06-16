

namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    public interface IUnitOfWork : IDisposable
    {
        Guid OperationId();

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
