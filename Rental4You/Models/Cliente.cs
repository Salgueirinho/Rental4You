using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }
        public int ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public ICollection<Reserva>? Reservas { get; set; }
    }
}
