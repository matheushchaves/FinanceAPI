

using System.ComponentModel.DataAnnotations;

namespace FinanceAPI.Models
{
    public class Usuario : Entidade
    {        
        [Required(ErrorMessage = "Email é obrigatório")]
        public string Email { get; set; } = "";
        [Required(ErrorMessage = "Senha é obrigatório")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")] 
        public string Nome { get; set; }

        public string Regra { get; set; } = "USER";

    }
}
