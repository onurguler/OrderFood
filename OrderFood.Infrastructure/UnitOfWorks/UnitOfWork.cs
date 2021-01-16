using OrderFood.Infrastructure.Repositories;
using System.Threading.Tasks;

namespace OrderFood.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DBContext _context;

        public UnitOfWork(DBContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IBaseRepository<TEntity, UKey> GetRepository<TEntity, UKey>() where TEntity : class
        {
            return new BaseRepository<TEntity, UKey>(_context);
        }

        public int Save()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
