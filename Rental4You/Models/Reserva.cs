using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; } 
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        public int? KilometrosInicio { get; set; }
        public int? KilometrosFim { get; set; }
        public bool? Estado { get; set; }

        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int? FuncionarioEntregaId { get; set; }
        public Funcionario? FuncionarioEntrega { get; set; }
        public int? FuncionarioRecebeId { get; set; }
        public Funcionario? FuncionarioRecebe { get; set; }
        public string? ObservacoesInicio { get; set; }
        public string? ObservacoesFim { get; set; }
      
    }
}
