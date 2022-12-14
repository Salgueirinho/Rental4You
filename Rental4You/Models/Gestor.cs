using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class Gestor
    {
        public int Id { get; set; }
        
        [PersonalData]
        public string Nome { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }

        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
