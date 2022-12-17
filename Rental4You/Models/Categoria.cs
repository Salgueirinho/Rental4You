using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name ="Nome da Categoria")]
        public string Nome { get; set; }
        [Display(Name ="Descrição da categoria")]
        [Required]
        public string Descricao { get; set; }
    }
}
