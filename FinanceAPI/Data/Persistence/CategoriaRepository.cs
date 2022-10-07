using FinanceAPI.Data.Context;
using FinanceAPI.Data.Persistence.Interfaces;
using FinanceAPI.Models;
using AppDataContext = FinanceAPI.Data.Context.AppDataContext;

namespace FinanceAPI.Data.Persistence
{
    public class CategoriaRepository: Repository<Categoria>,ICategoriaRepository
    {

        public CategoriaRepository(AppDataContext ctx) : base(ctx)    
        {

        }

        public new IQueryable<Categoria> AsQueryable()
        {
            return _context.Categorias.AsQueryable<Categoria>();
        }
    }
}
