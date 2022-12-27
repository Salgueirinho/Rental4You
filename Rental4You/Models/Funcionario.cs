using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }
        public string Nome { get; set; }
        //public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
    }
}
