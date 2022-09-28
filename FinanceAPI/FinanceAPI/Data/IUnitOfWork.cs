using FinanceAPI.Data.Persistence.Interfaces;

namespace FinanceAPI.Data
{
    public interface IUnitOfWork
    {
        IUsuarioRepository UsuarioRepository { get; }
        bool Commit();
        void Dispose();
    }
}
