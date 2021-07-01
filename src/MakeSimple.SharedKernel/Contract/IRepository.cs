namespace MakeSimple.SharedKernel.Contract
{
    using MakeSimple.SharedKernel.Wrappers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> : IDisposable
    {
        /// <summary>
        /// Unit Of Work Pattern
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Get row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync(object key, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get row by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get row by primary key and auto mapper to Model DTO
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="AutoMapperMappingException">Miss config Automapper</exception>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<IResponse<DTO>> FirstOrDefaultAsync<DTO>(object key, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get row by filter and auto mapper to Model DTO
        /// </summary>
        /// <param name="key"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="AutoMapperMappingException">Miss config Automapper</exception>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<IResponse<DTO>> FirstOrDefaultAsync<DTO>(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<List<TEntity>> ToListAsync(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
           , IPaginationQuery paging = null
           , params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database and auto mapper to Model DTO
        /// </summary>
        /// <typeparam name="DTO"></typeparam>
        /// <param name="paging"></param>
        /// <param name="orderBy"></param>
        /// <param name="expandSorts">Priority low</param>
        /// <param name="filters"></param>
        /// <param name="expandFilters">Priority low</param>
        /// <param name="includes"></param>
        /// <exception cref="AutoMapperMappingException">Miss config Automapper</exception>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<IPaginatedList<DTO>> ToListAsync<DTO>(
            IPaginationQuery paging
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , string expandSorts = null
            , IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , string expandFilters = null
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
        Task InsertRangeAsync(IList<TEntity> entities);

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