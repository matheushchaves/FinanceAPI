using FinanceAPI.Data.Persistence.Interfaces;

namespace FinanceAPI.Data
{
    public interface IUnitOfWork
    {
        IUsuarioRepository UsuarioRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        bool Commit();
        void Dispose();
    }
}
