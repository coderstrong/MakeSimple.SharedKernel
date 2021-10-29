namespace MakeSimple.SharedKernel.Contract
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<TContext, TEntity> : IDisposable
        where TContext : IUnitOfWork
        where TEntity : Entity
    {
        /// <summary>
        /// Unit Of Work Pattern
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Returns this object typed as System.Linq.IQueryable`1.
        /// This is a convenience method to help with disambiguation of extension methods
        /// in the same namespace that extend both interfaces.
        /// </summary>
        /// <returns> This object.</returns>
        IQueryable<TEntity> AsQueryable();

        /// <summary>
        /// Get row by primary key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get row by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get row by filter and auto mapper to Model DTO
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        /// <exception cref="AutoMapperMappingException">Miss config Automapper</exception>
        /// <exception cref="KeyNotFoundException">Miss config Automapper</exception>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<DTO> FindAsync<DTO>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get data from Database
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="orderBy"></param>
        /// <param name="paging"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="includes"></param>
        /// <returns></returns>
        Task<IList<TEntity>> ToListAsync(IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
           , PaginationQuery paging = null
           , CancellationToken cancellationToken = default
           , params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Get paginated data from Database and auto mapper to Model DTO
        /// </summary>
        /// <typeparam name="DTO"></typeparam>
        /// <param name="paging"></param>
        /// <param name="orderBy"></param>
        /// <param name="expandSorts">
        /// A comma delimited ordered list of property names to sort by. Adding a `-` before the name switches to sorting descendingly.
        /// Priority low
        /// </param>
        /// <param name="filters"></param>
        /// <param name="expandFilters">
        /// <param name="cancellationToken"></param>
        /// - A comma delimited list of fields to filter by formatted as `{Name}{Operator}{Value}` where
        ///     - {Name} is the name of a filterable property. You can also have multiple names (for OR logic) by enclosing them in brackets and using a pipe delimiter, eg. `(LikeCount|CommentCount)>10` asks if LikeCount or CommentCount is >10
        ///     - {Operator} is one of the Operators below
        ///     - {Value} is the value to use for filtering. You can also have multiple values (for OR logic) by using a pipe delimiter, eg.`Title@= new|hot` will return posts with titles that contain the text "new" or "hot"
        ///
        ///    | Operator | Meaning                       | Operator  | Meaning                                      |
        ///    | -------- | ----------------------------- | --------- | -------------------------------------------- |
        ///    | `==`     | Equals                        |  `!@=`    | Does not Contains                            |
        ///    | `!=`     | Not equals                    |  `!_=`    | Does not Starts with                         |
        ///    | `>`      | Greater than                  |  `@=*`    | Case-insensitive string Contains             |
        ///    | `&lt;`   | Less than                     |  `_=*`    | Case-insensitive string Starts with          |
        ///    | `>=`     | Greater than or equal to      |  `==*`    | Case-insensitive string Equals               |
        ///    | `&lt;=`  | Less than or equal to         |  `!=*`    | Case-insensitive string Not equals           |
        ///    | `@=`     | Contains                      |  `!@=*`   | Case-insensitive string does not Contains    |
        ///    | `_=`     | Starts with                   |  `!_=*`   | Case-insensitive string does not Starts with |
        /// - Priority low</param>
        /// <param name="includes"></param>
        /// <exception cref="AutoMapperMappingException">Miss config Automapper</exception>
        /// <exception cref="NullReferenceException">Param Paging is required has value</exception>
        Task<IList<DTO>> ToListAsync<DTO>(
            PaginationQuery paging
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , string expandSorts = null
            , IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , string expandFilters = null
            , CancellationToken cancellationToken = default
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
        /// <param name="cancellationToken"></param>
        Task InsertRangeAsync(IList<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update data to database
        /// </summary>
        /// <param name="entity"></param>
        void Update(TEntity entity);

        /// <summary>
        /// Update range to database
        /// </summary>
        /// <param name="entities"></param>
        void UpdateRange(IList<TEntity> entities);

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cancellationToken"></param>
        Task DeleteAsync(object key, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);

        /// <summary>
        /// Delete data in database
        /// </summary>
        /// <param name="entities"></param>
        void DeleteRange(ICollection<TEntity> entities);

        /// <summary>
        /// Check exist record on database by filter
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default);
    }
}