﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Rental4You.Models;

namespace Rental4You.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Empresa> Empresas { get; set; }
        public DbSet<Gestor> Gestores { get; set; }
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}