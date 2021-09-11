namespace MakeSimple.SharedKernel.Infrastructure.Repository
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Exceptions;
    using MakeSimple.SharedKernel.Utils;
    using MakeSimple.SharedKernel.Wrappers;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class EfRepositoryGeneric<TContext, TEntity> : Disposable, IRepository<TContext, TEntity>
        where TContext : DbContext, IUnitOfWork
        where TEntity : Entity
    {
        private readonly TContext _context;
        private readonly SieveProcessor _sieveProcessor;
        private readonly IMapper _mapper;

        public EfRepositoryGeneric(TContext context
            , SieveProcessor sieveProcessor
            , IMapper mapper)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public virtual async Task DeleteAsync(object key, CancellationToken cancellationToken = default)
        {
            var entity = await _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).ConfigureAwait(false);
            if (entity != null)
                _context.Set<TEntity>().Remove(entity);
        }

        public async Task<IList<TEntity>> ToListAsync(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, PaginationQuery paging = null
           , CancellationToken cancellationToken = default
           , params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);

                if (paging != null)
                {
                    query = query.Skip(paging.Skip()).Take(paging.PageSize);
                }
            }
            else if (paging != null)
            {
                query = query.OrderByDescending(e => e.CreatedAt);
                query = query.Skip(paging.Skip()).Take(paging.PageSize);
            }

            return await query.AsNoTracking().ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<IList<DTO>> ToListAsync<DTO>(
            PaginationQuery paging
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , string expandSorts = null
            , IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , string expandFilters = null
            , CancellationToken cancellationToken = default
            , params Expression<Func<TEntity, object>>[] includes)
        {
            Guard.NotNull(paging, nameof(paging));

            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            if (!string.IsNullOrEmpty(expandFilters))
            {
                SieveModel expandFilter = new SieveModel
                {
                    Filters = expandFilters
                };
                query = _sieveProcessor.Apply(expandFilter, query, applySorting: false, applyPagination: false);
            }

            paging.TotalItems = query.Count();

            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else if (string.IsNullOrEmpty(expandSorts))
            {
                query = query.OrderByDescending(e => e.CreatedAt);
            }

            if (!string.IsNullOrEmpty(expandSorts))
            {
                SieveModel expandSort = new SieveModel
                {
                    Sorts = expandSorts
                };
                query = _sieveProcessor.Apply(expandSort, query, applyFiltering: false, applyPagination: false);
            }

            query = query.Skip(paging.Skip()).Take(paging.PageSize);

            if (paging.TotalItems > 0)
            {
                return await query.AsNoTracking().ProjectTo<DTO>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken).ConfigureAwait(false);
            }

            return new List<DTO>();
        }

        public async Task<TEntity> FirstOrDefaultAsync(object key, CancellationToken cancellationToken = default)
        {
            Guard.NotNull(key, nameof(key));

            return await _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            Guard.NotNull(filter, nameof(filter));

            var query = _context.Set<TEntity>().Where(filter);
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task<DTO> FindAsync<DTO>(object key, CancellationToken cancellationToken = default)
        {
            Guard.NotNull(key, nameof(key));

            var item = await _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).ConfigureAwait(false);

            if (item != null)
            {
                return _mapper.Map<DTO>(item);
            }
            else
            {
                throw new NotFoundException(Error.Created("db#001", $"FindAsync not found item with key {key}"));
            }
        }

        public async Task<DTO> FindAsync<DTO>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes)
        {
            Guard.NotNull(filter, nameof(filter));

            var query = _context.Set<TEntity>().AsNoTracking().Where(filter);
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            var item = await query.ProjectTo<DTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (item != null)
            {
                return item;
            }
            else
            {
                throw new NotFoundException(Error.Created("db#002", $"FindAsync<DTO> not found item with filter"));
            }
        }

        public TEntity Insert(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public async Task InsertRangeAsync(IList<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
        {
            return await _context.Set<TEntity>().AnyAsync(filter, cancellationToken).ConfigureAwait(false);
        }
    }
}