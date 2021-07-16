namespace MakeSimple.SharedKernel.Contract
{
    public interface IRepository<TContext, TEntity> : IRepositoryCore<TEntity>
        where TContext : IUnitOfWork
        where TEntity : ModelShared
    {
    }
}