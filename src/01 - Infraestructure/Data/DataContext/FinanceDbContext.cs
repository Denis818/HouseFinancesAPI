using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext.Context
{
    public partial class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<Member> Membros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

      /*  protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Categoria) // Define a propriedade de navegação na classe Despesa.
                .WithMany() // Aqui você pode especificar uma propriedade de coleção na Categoria se houver. Se não, deixe vazio.
                .HasForeignKey(d => d.CategoriaId); // Define a chave estrangeira na classe Despesa.
        }*/
    }
}
