namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepositoryGeneric<TContext, TEntity> : IDisposable where TContext : IUnitOfWork
    {
        /// <summary>
        /// Unit Of Work Pattern
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Get row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes">Not support then include</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefaultAsync(object key, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get row by primary key and auto mapper to Model DTO
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes">Not support then include</param>
        /// <returns></returns>
        /// <exception cref="">Miss config Automapper</exception>
        Task<DTO> FirstOrDefaultAsync<DTO>(object key, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="includes">Not support then include</param>
        /// <returns></returns>
        public Task<List<TEntity>> ToList(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
           , IPaginationQuery paging = null
           , params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database and auto mapper to Model DTO
        /// </summary>
        /// <typeparam name="DTO"></typeparam>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="includes">Not support then include</param>
        /// <returns></returns>
        /// <exception>Miss config Automapper</exception>
        public Task<List<DTO>> ToList<DTO>(
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