namespace MakeSimple.SharedKernel.Infrastructure.Repository
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Infrastructure.DTO;
    using MakeSimple.SharedKernel.Utils;
    using MakeSimple.SharedKernel.Wrappers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Sieve.Models;
    using Sieve.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class EfAuditUuidRepositoryGeneric<TContext, TEntity> : Disposable, IAuditRepositoryGeneric<TContext, TEntity>
        where TContext : DbContext, IUnitOfWork
        where TEntity : AuditEntity<Guid>
    {
        private readonly TContext _context;
        private readonly SieveProcessor _sieveProcessor;
        private readonly IMapper _mapper;
        private readonly ClaimsPrincipal _user;

        public EfAuditUuidRepositoryGeneric(TContext context
            , SieveProcessor sieveProcessor
            , IMapper mapper
            , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _sieveProcessor = sieveProcessor;
            _mapper = mapper;
            _user = httpContextAccessor.HttpContext.User == null ? new ClaimsPrincipal(
                new List<ClaimsIdentity>()
                {
                    new ClaimsIdentity(new List<Claim>(){
                        new Claim(ClaimTypes.Name, "Anonymous")
                    })
                }) : httpContextAccessor.HttpContext.User;
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return _context;
            }
        }

        public virtual void Delete(object key)
        {
            var entity = _context.Set<TEntity>().Find(key);
            if (entity != null)
                _context.Set<TEntity>().Remove(entity);
        }

        public async Task<List<TEntity>> ToListAsync(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
           , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IPaginationQuery paging = null
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
                    query = query.Skip(paging.Skip).Take(paging.PageSize);
                }
            }
            else if (paging != null)
            {
                query = query.OrderByDescending(e => e.Id);
                query = query.Skip(paging.Skip).Take(paging.PageSize);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IPaginatedList<DTO>> ToListAsync<DTO>(
            IPaginationQuery paging
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , string expandSorts = null
            , IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , string expandFilters = null
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

            var totalItems = query.Count();

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
                query = query.OrderByDescending(e => e.Id);
            }

            if (!string.IsNullOrEmpty(expandSorts))
            {
                SieveModel expandSort = new SieveModel
                {
                    Sorts = expandSorts
                };
                query = _sieveProcessor.Apply(expandSort, query, applyFiltering: false, applyPagination: false);
            }

            query = query.Skip(paging.Skip).Take(paging.PageSize);

            if (totalItems > 0)
            {
                return new PaginatedList<DTO>(
                    await query.AsNoTracking().ProjectTo<DTO>(_mapper.ConfigurationProvider).ToListAsync().ConfigureAwait(false)
                    , totalItems
                    , paging.PageNumber
                    , paging.PageSize
                    );
            }
            else
            {
                return new PaginatedList<DTO>(HttpStatusCode.NotFound);
            }
        }

        public async Task<TEntity> FirstOrDefaultAsync(object key, params Expression<Func<TEntity, object>>[] includes)
        {
            Guard.NotNull(key, nameof(key));

            var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key));
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
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
            return await query.FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<IResponse<DTO>> FirstOrDefaultAsync<DTO>(object key, params Expression<Func<TEntity, object>>[] includes)
        {
            Guard.NotNull(key, nameof(key));

            var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key));
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return new Response<DTO>(await query.ProjectTo<DTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync().ConfigureAwait(false));
        }

        public async Task<IResponse<DTO>> FirstOrDefaultAsync<DTO>(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes)
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
            return new Response<DTO>(await query.ProjectTo<DTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync().ConfigureAwait(false));
        }

        public TEntity Insert(TEntity entity)
        {
            entity.CreatedBy = _user.Identity.Name;
            entity.CreatedAt = DateTime.UtcNow;

            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public async Task InsertRangeAsync(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedBy = _user.Identity.Name;
                entity.CreatedAt = DateTime.UtcNow;
            }

            await _context.Set<TEntity>().AddRangeAsync(entities).ConfigureAwait(false);
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedBy = _user.Identity.Name;
            entity.ModifiedAt = DateTime.UtcNow;

            _context.Set<TEntity>().Update(entity);
        }
    }
}