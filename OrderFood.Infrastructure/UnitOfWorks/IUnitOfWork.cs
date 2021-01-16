using OrderFood.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace OrderFood.Infrastructure.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<TEntity, UKey> GetRepository<TEntity, UKey>() where TEntity : class;
        int Save();
        Task<int> SaveAsync();
    }
}
