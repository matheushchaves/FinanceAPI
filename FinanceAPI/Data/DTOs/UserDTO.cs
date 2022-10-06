using FinanceAPI.Models;
namespace FinanceAPI.Data.DTOs
{
    public class UserDTO: Entidade
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Regra { get; set; }
        


    }
}
