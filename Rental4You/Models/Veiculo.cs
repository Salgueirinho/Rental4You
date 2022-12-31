
using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Veiculo
    {
        [Key]
        public int Id { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Localizacao { get; set; }
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }
        [Display(Name = "Número de Lugares")]
        public int NumeroLugares { get; set; }

        //true é manual. false é automatico
        public bool Caixa { get; set; }
        public bool Disponivel { get; set; }
        public decimal Custo { get; set; }
        public bool Danos { get; set; }
        [Display(Name = "Quilómetros")]
        public int Kilometros { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
    }
}
