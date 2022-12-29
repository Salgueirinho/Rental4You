using Rental4You.Models;
using System.ComponentModel.DataAnnotations;

namespace Rental4You.ViewModels
{
    public class PesquisaVeiculosViewModel
    {
        public List<Veiculo> ListaVeiculos { get; set; }
        public int NumResultados { get; set; }

        //[Display(Name = "PESQUISA DE VEÍCULOS ...", Prompt = "introduza ")]
        public string Localizacao { get; set; }
        public int CategoriaId { get; set; }

        [Display(Name = "Data de Levantamento", Prompt = "dd-mm-yyyy")]
        [DataType(DataType.Date)]
        public DateTime DataLevantamento { get; set; }
        [Display(Name = "Data de Entrega", Prompt = "dd-mm-yyyy")]
        [DataType(DataType.Date)]
        public DateTime DataEntrega { get; set; }

    }
}
