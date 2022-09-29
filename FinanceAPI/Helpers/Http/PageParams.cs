using FinanceAPI.Models;

namespace FinanceAPI.Helpers.Http
{
    public class PageParams
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string Filter { get; set; } = "";

        public string Orderby { get; set; } = "";

        public Guid ?Id { get; set; }
        public DateTimeOffset ?DataCriacao { get; set; }
        public DateTimeOffset ?DataAlteracao { get; set; }
        public bool ?Bloqueado { get; set; }

    }
}
