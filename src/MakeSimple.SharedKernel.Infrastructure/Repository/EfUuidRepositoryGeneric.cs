namespace MakeSimple.SharedKernel.Repository
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MakeSimple.SharedKernel.Contract;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public class EfUuidRepositoryGeneric<TContext, TEntity> : Disposable, IRepositoryGeneric<TContext, TEntity>
        where TContext : DbContext, IUnitOfWork
        where TEntity : UuidEntity
    {
        private readonly TContext _context;
        private readonly IMapper _mapper;

        public EfUuidRepositoryGeneric(TContext context
            , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public async Task<List<TEntity>> ToList(
           IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , IPaginationQuery paging = null
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

        public async Task<List<DTO>> ToList<DTO>(
            IEnumerable<Expression<Func<TEntity, bool>>> filters = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , IPaginationQuery paging = null
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

            return await query.AsNoTracking().ProjectTo<DTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<TEntity> FirstOrDefaultAsync(object key, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key));
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<DTO> FirstOrDefaultAsync<DTO>(object key, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key));
            if (includes != null && includes.Length > 0)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return await query.ProjectTo<DTO>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public TEntity Insert(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public async Task InsertRangeAsync(IList<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedAt = DateTime.Now;

            _context.Set<TEntity>().Update(entity);
        }
    }
}