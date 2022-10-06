

using FinanceAPI.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceAPI.Models
{
    public class Usuario : Entidade
    {        
        [Required(ErrorMessage = "Email é obrigatório")]
        public string Email { get; set; } = "";
                
        [DataType(DataType.Password)]
        public virtual string SenhaSalva
        {
            get;
            set;
        }

        [NotMapped]
        [Required(ErrorMessage = "Senha é obrigatório")]
        public string Senha
        {
            
            get { return EncryptionHelper.Decrypt(SenhaSalva); }
            set { SenhaSalva = EncryptionHelper.Encrypt(value); }
        }


        [Required(ErrorMessage = "Nome é obrigatório")] 
        public string Nome { get; set; }

        public string Regra { get; set; } = "USER";

    }
}
