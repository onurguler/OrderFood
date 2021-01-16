using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderFood.Infrastructure.Repositories
{
    public interface IBaseRepository<TEntity, UKey> where TEntity : class
    {
        TEntity Get(UKey id);
        ValueTask<TEntity> GetAsync(UKey id);
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null);
        bool IsExists(Expression<Func<TEntity, bool>> filter = null);
        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> filter = null);

        TEntity GetFirst(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        TEntity GetOne(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "");

        IQueryable<TEntity> GetQueryable<Y>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, Y>> orderBy = null, bool orderByAscending = true);
        List<TEntity> PagedGetAll<X>(int pageSize, int pageNum, out int totalRecords, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, X>> orderBy = null, bool orderByAscending = true);
        Task<List<TEntity>> PagedGetAllAsync<X>(int pageSize, int pageNum, out int totalRecords, Expression<Func<TEntity, bool>> predicate = null, Expression<Func<TEntity, X>> orderBy = null, bool orderByAscending = true);

        Task<List<TEntity>> GetListAsync<Y>(Expression<Func<TEntity, bool>> filter = null, Expression<Func<TEntity, Y>> orderby = null, bool orderbyAscending = true);

        IQueryable<TEntity> GetQueryableFromSql(string sql);
    }
}
