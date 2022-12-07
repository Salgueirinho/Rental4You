﻿using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class Funcionario
    {
        [Key]
        public int Id { get; set; }

        public int IdApplicationUser { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

    }
}
