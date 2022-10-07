using FinanceAPI.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FinanceAPI.Data.Context
{
    public class AppDataContext : DbContext
    {
        public AppDataContext(DbContextOptions<AppDataContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {        
                Id = new Guid("79aed679-3622-42ae-8f59-1dd31cede462"),
                Nome = "Administrador",
                Email = "matheushchaves@gmail.com",
                Senha = "1Qaz!@#",
                Regra = "ADMIN"
            });

            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<Categoria>()
               .HasOne<Usuario>(c=> c.Usuario)
               .WithMany(c => c.Categorias)               
               .IsRequired(); ;
        }

    }
}
