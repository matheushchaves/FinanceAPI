using FinanceAPI.Data.Context;
using FinanceAPI.Data.Persistence.Interfaces;
using FinanceAPI.Models;
using AppDataContext = FinanceAPI.Data.Context.AppDataContext;

namespace FinanceAPI.Data.Persistence
{
    public class UsuarioRepository: Repository<Usuario>,IUsuarioRepository
    {

        public UsuarioRepository(AppDataContext ctx) : base(ctx)    
        {

        }

        public new IQueryable<Usuario> AsQueryable()
        {
            return _context.Usuarios.AsQueryable<Usuario>();
        }
    }
}
