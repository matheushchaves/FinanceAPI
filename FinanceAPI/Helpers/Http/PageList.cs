using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Helpers.Http
{
    public class PageList<T>
    {
        public List<T> items { get; set; }
        public bool hasNext { get; set; } = false;

        public async static Task<PageList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PageList<T>()
            {
                items = items, 
                hasNext = pageNumber < totalPages
            };
        }
    }
}
