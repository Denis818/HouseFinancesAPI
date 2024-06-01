using Domain.Models.Categorias;
using Domain.Models.Despesas;
using Domain.Models.Membros;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext
{
    public partial class FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : DbContext(options)
    {
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<GrupoFatura> GrupoFaturas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Membro> Membros { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);
        }

        public void SetConnectionString(string newStringConnection)
        {
            Database.CurrentTransaction?.Commit();

            Database.SetConnectionString(newStringConnection);
        }
    }
}
