namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepositoryGeneric<TContext, TEntity> : IDisposable where TContext : IUnitOfWork
    {
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Get row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<TEntity> GetOneAsync(object key);

        /// <summary>
        /// Get row by primary key and auto mapper to Model M
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="">Miss config Automapper</exception>
        Task<M> GetOneAsync<M>(object key);

        /// <summary>
        /// Get data from Database
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        public Task<List<TEntity>> GetAllAsync(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IPaginationQuery paging = null
           , params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database and auto mapper to Model M
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="">Miss config Automapper</exception>
        public Task<List<M>> GetAllAsync<M>(
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , IPaginationQuery paging = null
            , params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Insert data to database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert(TEntity entity);

        /// <summary>
        /// Insert range to database
        /// </summary>
        /// <param name="entities"></param>
        void InsertRange(IList<TEntity> entities);

        /// <summary>
        /// Update data to database
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Delete data to database
        /// </summary>
        /// <param name="key"></param>
        void Delete(object key);
    }
}