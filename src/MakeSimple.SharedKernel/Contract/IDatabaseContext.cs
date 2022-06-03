namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IDatabaseContext : IDisposable
    {
        string Uuid { get; }

        Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    }
}