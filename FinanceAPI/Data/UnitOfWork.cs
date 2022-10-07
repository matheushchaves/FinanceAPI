using FinanceAPI.Data.Context;
using FinanceAPI.Data.Persistence;
using FinanceAPI.Data.Persistence.Interfaces;

namespace FinanceAPI.Data
{
    public class UnitOfWork: IUnitOfWork
    {
        public AppDataContext adc;        

        public UnitOfWork(AppDataContext adc)
        {
            this.adc = adc;            
        }

        public bool Commit()
        {
            return adc.SaveChanges() > 0;
        }

        public void Dispose()
        {
            adc.Dispose();
        }

        private IUsuarioRepository usuarioRepository;
        public IUsuarioRepository UsuarioRepository
        {
            get { return usuarioRepository = usuarioRepository ?? new UsuarioRepository(adc); }
        }

        private ICategoriaRepository categoriaRepository;
        public ICategoriaRepository CategoriaRepository
        {
            get { return categoriaRepository = categoriaRepository ?? new CategoriaRepository(adc); }
        }




    }
}
