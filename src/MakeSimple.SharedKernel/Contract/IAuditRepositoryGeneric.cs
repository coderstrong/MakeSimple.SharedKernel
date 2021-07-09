namespace MakeSimple.SharedKernel.Contract
{
    public interface IAuditRepositoryGeneric<TContext, TEntity> : IRepository<TEntity>
        where TContext : IUnitOfWork
        where TEntity : AuditEntityShared
    {
    }
}