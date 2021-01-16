using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.Infrastructure.Repositories
{
    class BaseRepository<TEntity, UKey> : IDisposable, IBaseRepository<TEntity, UKey> where TEntity : class
    {
        private readonly DBContext _context;

        public BaseRepository(DBContext context)
        {
            _context = context;
        }

        public virtual void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public virtual Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public virtual TEntity Get(UKey id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public virtual ValueTask<TEntity> GetAsync(UKey id)
        {
            return _context.Set<TEntity>().FindAsync(id);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Count();
        }

        public virtual Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).CountAsync();
        }

        public virtual TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return GetQueryable(filter, orderBy, includeProperties).FirstOrDefault();
        }

        public virtual Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            return GetQueryable(filter, orderBy, includeProperties).FirstOrDefaultAsync();
        }

        public virtual Task<List<TEntity>> GetListAsync<Y>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, Y>> orderby = null, bool orderbyAscending = true)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null) query = query.Where(filter);

            if (orderby != null)
            {
                if (orderbyAscending)
                {
                    query = query.OrderBy(orderby);
                }
                else
                {
                    query = query.OrderByDescending(orderby);
                }
            }

            return query.ToListAsync();
        }

        public virtual TEntity GetOne(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            return GetQueryable(filter, null, includeProperties).SingleOrDefault();
        }

        public virtual Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            return GetQueryable(filter, null, includeProperties).SingleOrDefaultAsync();
        }

        public virtual IQueryable<TEntity> GetQueryable<Y>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, Y>> orderBy = null, bool orderByAscending = true)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (filter != null) query = query.Where(filter);
            if (orderBy != null)
            {
                if (orderByAscending)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            return query;
        }

        public virtual IQueryable<TEntity> GetQueryableFromSql(string sql)
        {
            return _context.Set<TEntity>().FromSqlRaw(sql);
        }

        public virtual bool IsExists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).Any();
        }

        public virtual Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(filter).AnyAsync();
        }

        public virtual List<TEntity> PagedGetAll<X>(int pageSize, int pageNum, out int totalRecords, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, X>> orderBy = null, bool orderByAscending = true)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
            {
                if (orderByAscending)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            totalRecords = query.Count();

            return query.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToList();
        }

        public virtual Task<List<TEntity>> PagedGetAllAsync<X>(int pageSize, int pageNum, out int totalRecords, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, X>> orderBy = null, bool orderByAscending = true)
        {
            var query = _context.Set<TEntity>().AsQueryable();

            if (predicate != null) query = query.Where(predicate);

            if (orderBy != null)
            {
                if (orderByAscending)
                {
                    query = query.OrderBy(orderBy);
                }
                else
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            totalRecords = query.Count();

            return query.Skip(pageSize * (pageNum - 1)).Take(pageSize).ToListAsync();
        }

        public virtual void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AttachRange(entities);
            _context.Entry(entities).State = EntityState.Modified;        }

        protected virtual IQueryable<TEntity> GetQueryable(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null)
        {
            includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }
    }
}
