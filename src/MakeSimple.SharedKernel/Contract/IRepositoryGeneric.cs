namespace MakeSimple.SharedKernel.Repository
{
    using MakeSimple.SharedKernel.Contract;

    public interface IRepositoryGeneric<TContext, TEntity> : IRepository<TEntity>
        where TContext : IUnitOfWork
    {
        
    }
}