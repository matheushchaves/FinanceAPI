namespace FinanceAPI.Data.Persistence.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        void RemoveRange<T>(IEnumerable<T> entity) where T : class;
        IQueryable<T> AsQueryable();
        bool SaveChanges();
    }
}
