namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork : IDisposable
    {
        IQueryable<TEntity> Entity<TEntity>() where TEntity : Entity;

        /// <summary>
        /// Get row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync<TEntity>(object key, CancellationToken cancellationToken = default) where TEntity : Entity;

        /// <summary>
        /// Get row by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes) where TEntity : Entity;

        /// <summary>
        /// Insert data to database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity Insert<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Insert range to database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        Task InsertRangeAsync<TEntity>(IList<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity;

        /// <summary>
        /// Update data to database
        /// </summary>
        /// <param name="entity"></param>
        void Update<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Update range to database
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange<TEntity>(IList<TEntity> entities) where TEntity : Entity;

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteAsync<TEntity>(object key, CancellationToken cancellationToken = default) where TEntity : Entity;

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="entity"></param>
        void Delete<TEntity>(TEntity entity) where TEntity : Entity;

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="entities"></param>
        void DeleteRange<TEntity>(ICollection<TEntity> entities) where TEntity : Entity;

        /// <summary>
        /// Check exist record on database by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : Entity;
    }
}