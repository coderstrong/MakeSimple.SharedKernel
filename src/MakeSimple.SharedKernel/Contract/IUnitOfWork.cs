namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        string Uuid { get; }

        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}