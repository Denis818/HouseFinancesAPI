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
    }
}
