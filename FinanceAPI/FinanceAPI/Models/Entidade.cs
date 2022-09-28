using Microsoft.EntityFrameworkCore.Storage.Internal;
using System.ComponentModel.DataAnnotations;

namespace FinanceAPI.Models
{
    public class Entidade
    {
        [Key]
        public Guid Id { get; set; }
        public DateTimeOffset Datacriacao { get; set; }
        public DateTimeOffset Dataalteracao { get; set; }
        public bool Bloqueado { get; set; } = false;

    }
}
