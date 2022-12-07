using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class Gestor
    {
        public int Id { get; set; }
        
        [PersonalData]
        public string Nome { get; set; }

        public Empresa empresa { get; set; }


    }
}
