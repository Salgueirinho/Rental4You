﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Rental4You.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string ?PrimeiroNome { get; set; }

        [PersonalData]
        public string ?UltimoNome { get; set; }

        [PersonalData]
        public int Idade { get; set; }

        [PersonalData]
        public int NIF { get; set; }

    }
}
