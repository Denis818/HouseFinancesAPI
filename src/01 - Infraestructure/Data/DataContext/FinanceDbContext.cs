using Data.EntitesMaps.FinanceDataBase.Finance;
using Data.EntitesMaps.FinanceDataBase.User;
using Domain.Models.Finance;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext.Context
{
    public partial class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
    {
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<Membro> Membros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permissao> Permissoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new DespesaMap());
            modelBuilder.ApplyConfiguration(new CategoriaMap());
            modelBuilder.ApplyConfiguration(new MembroMap());

            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new PermissaoMap());
        }
    }
}
