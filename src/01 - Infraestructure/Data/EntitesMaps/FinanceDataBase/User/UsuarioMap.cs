using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Users;

namespace Data.EntitesMaps.FinanceDataBase.User
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.Property(u => u.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(u => u.Email).HasColumnType("varchar(30)").IsRequired();
            builder.Property(u => u.Password).HasColumnType("varchar(100)").IsRequired();
            builder.Property(u => u.Salt).HasColumnType("varchar(100)").IsRequired();

            builder.HasMany(u => u.Permissoes)
                   .WithMany(p => p.Usuarios);

            builder.HasIndex(u => u.Id).HasDatabaseName("IX_Usuarios_Id");
            builder.HasIndex(u => u.Email).HasDatabaseName("IX_Email");
        }
    }
}