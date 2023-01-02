using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Reserva
    {
        [Key]
        public int Id { get; set; } 
        public int VeiculoId { get; set; }
        public Veiculo? Veiculo { get; set; }
        [Display(Name = "Quilómetros Iniciais")]
        public int? KilometrosInicio { get; set; }
        [Display(Name = "Quilómetros Finais")]
        public int? KilometrosFim { get; set; }
        public bool? Estado { get; set; }
        public bool? Confirmado { get; set; }
        [Display(Name = "Data de Inicio", Prompt = "dd-mm-yyyy")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; }
        [Display(Name = "Data de Fim", Prompt = "dd-mm-yyyy")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        public int? FuncionarioEntregaId { get; set; }
        [Display(Name = "Funcionário de Entrega")]
        public Funcionario? FuncionarioEntrega { get; set; }
        public int? FuncionarioRecebeId { get; set; }
        [Display(Name = "Funcionário que Recebeu")]
        public Funcionario? FuncionarioRecebe { get; set; }
        [Display(Name = "Observações Iniciais")]
        public string? ObservacoesInicio { get; set; }
        [Display(Name = "Observações Finais")]
        public string? ObservacoesFim { get; set; }

        [DataType(DataType.Date)]
        public DateTime DataConfirmada { get; set; }

        [Display(Name = "Custo")]
        public decimal Valor { get;set; }
    }
}
