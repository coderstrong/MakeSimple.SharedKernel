namespace MakeSimple.SharedKernel.Repository
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using MakeSimple.SharedKernel.Contract;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class EfAuditUuidRepositoryGeneric<TContext, TEntity> : Disposable, IRepositoryGeneric<TContext, TEntity>
        where TContext : DbContext, IUnitOfWork
        where TEntity : AuditUuidEntity
    {
        private readonly TContext _context;
        private readonly IMapper _mapper;
        private readonly ClaimsPrincipal _user;
        public EfAuditUuidRepositoryGeneric(TContext context
            , IMapper mapper
            , IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
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

        public async Task<List<TEntity>> GetAllAsync(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, IPaginationQuery paging = null
            , params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
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

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<List<M>> GetAllAsync<M>(
            Expression<Func<TEntity, bool>> filter = null
            , Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null
            , IPaginationQuery paging = null
            , params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
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

            return await query.AsNoTracking().ProjectTo<M>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<TEntity> GetOneAsync(object key)
        {
            return await _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key)).FirstOrDefaultAsync();

        }

        public async Task<M> GetOneAsync<M>(object key)
        {
            return await _context.Set<TEntity>().AsNoTracking().Where(e => e.Id.Equals(key)).ProjectTo<M>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        }

        public TEntity Insert(TEntity entity)
        {
            entity.CreatedBy = _user.Identity.Name;
            entity.CreatedAt = DateTime.Now;

            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public void InsertRange(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.CreatedBy = _user.Identity.Name;
                entity.CreatedAt = DateTime.Now;
            }

            _context.Set<TEntity>().AddRange(entities);
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedBy = _user.Identity.Name;
            entity.ModifiedAt = DateTime.Now;

            _context.Set<TEntity>().Update(entity);
        }
    }
}