using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceAPI.Models
{
    public class Categoria: Entidade
    {
        public Guid UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatório")]
        public string Descricao { get; set; }
        [Required(ErrorMessage = "Ícone é obrigatório")]
        public string Icone { get; set; }
    }
}
