using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [PersonalData]
        public string Nome { get; set; }
        public ICollection<Gestor> Gestores { get; set; }
        public bool EstadoSubscricao { get; set; }
        [Display(Name = "Avaliação")]
        [Range(0,10, ErrorMessage = "Valores entre 0 e 10")]
        public decimal Avalicao { get; set; }
        public ICollection<Veiculo>? Veiculos { get; set; }
        public ICollection<Funcionario>? Funcionarios { get; set; }
    }
}
