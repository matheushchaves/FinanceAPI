using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace FinanceAPI.Helpers.Http
{
    public class PageList<T,D>
    {
        public List<D> items { get; set; }
        public bool hasNext { get; set; } = false;

        public async static Task<PageList<T,D>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize, IMapper mapper)
        {
            var count = source.Count();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            var itemsDTO = mapper.Map<List<D>>(items);
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);

            return new PageList<T,D>()
            {
                items = itemsDTO, 
                hasNext = pageNumber < totalPages
            };
        }
    }
}
