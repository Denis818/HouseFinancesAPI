using Domain.Models.Finance;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.DataContext.Context
{
    public partial class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
    {
        #region Finance
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<Membro> Membros { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        #endregion

        #region Usuário
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Permissao> UsuarioPermissoes { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Permissao>()
          .HasOne(up => up.Usuario)
          .WithMany(u => u.Permissoes)
          .HasForeignKey(up => up.UsuarioId);

        }
    }
}
