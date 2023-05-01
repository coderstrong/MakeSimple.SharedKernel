namespace MakeSimple.SharedKernel.Infrastructure.Repository
{
    using MakeSimple.SharedKernel.Contract;
    using MakeSimple.SharedKernel.Utils;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    public class EfUnitOfWork<TContext> : Disposable, IUnitOfWork<TContext>
        where TContext : DbContext, IDatabaseContext
    {
        private readonly TContext _context;

        public string Uuid => Guid.NewGuid().ToString();

        public EfUnitOfWork(TContext context)
        {
            _context = context;
        }

        public virtual async Task DeleteAsync<TEntity>(object key, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            var entity = await _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).ConfigureAwait(false);
            if (entity != null)
                _context.Set<TEntity>().Remove(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : Entity
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void DeleteRange<TEntity>(ICollection<TEntity> entities) where TEntity : Entity
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(object key, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            Guard.NotNull(key, nameof(key));

            return await _context.Set<TEntity>().FindAsync(new object[] { key }, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TEntity> FirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] includes) where TEntity : Entity
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

        public TEntity Insert<TEntity>(TEntity entity) where TEntity : Entity
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public async Task InsertRangeAsync<TEntity>(IList<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            await _context.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            _context.Set<TEntity>().Update(entity);
        }

        public void UpdateRange<TEntity>(IList<TEntity> entities) where TEntity : Entity
        {
            _context.Set<TEntity>().UpdateRange(entities);
        }

        public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default) where TEntity : Entity
        {
            return await _context.Set<TEntity>().AnyAsync(filter, cancellationToken).ConfigureAwait(false);
        }

        public IQueryable<TEntity> Entity<TEntity>() where TEntity : Entity
        {
            return _context.Set<TEntity>().AsQueryable();
        }

        public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}