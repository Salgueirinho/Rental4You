using Microsoft.AspNetCore.Identity;

namespace Rental4You.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [PersonalData]
        public string Nome { get; set; }
        public ICollection<Gestor> Gestores { get; set; }
        public bool EstadoSubscricao { get; set; }
        public decimal Avalicao { get; set; }
        public ICollection<Veiculo>? Veiculos { get; set; }
    }
}
