using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [PersonalData]
        public string Nome { get; set; }

        public Gestor gestor { get; set; }
        public int GestorId { get; set; }
    }
}
