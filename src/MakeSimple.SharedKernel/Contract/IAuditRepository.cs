namespace MakeSimple.SharedKernel.Contract
{
    public interface IAuditRepository<TContext, TEntity> : IRepositoryCore<TEntity>
        where TContext : IUnitOfWork
        where TEntity : AuditModelShared
    {
    }
}