using FinanceAPI.Data.Persistence.Interfaces;
using AppDataContext = FinanceAPI.Data.Context.AppDataContext;

namespace FinanceAPI.Data.Persistence
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDataContext _context;
        public Repository(AppDataContext context)
        {
            _context = context;
        }

        public void Insert(T entity)
        {
            _context.Add(entity);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Remove(entity);
        }
        public void RemoveRange<T>(IEnumerable<T> entity) where T : class
        {
            _context.RemoveRange(entity);
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() > 0;
        }

        public IQueryable<T> AsQueryable()
        {
            throw new NotImplementedException();
        }
    }

}
