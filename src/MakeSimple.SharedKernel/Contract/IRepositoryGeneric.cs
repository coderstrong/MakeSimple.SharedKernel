namespace MakeSimple.SharedKernel.Contract
{
    public interface IRepositoryGeneric<TContext, TEntity> : IRepository<TEntity>
        where TContext : IUnitOfWork
        where TEntity : EntityShared
    {
    }
}