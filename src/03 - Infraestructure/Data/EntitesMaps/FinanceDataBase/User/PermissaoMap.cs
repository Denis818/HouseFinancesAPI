using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Users;

namespace Data.EntitesMaps.FinanceDataBase.User
{
    public class PermissaoMap : IEntityTypeConfiguration<Permissao>
    {
        public void Configure(EntityTypeBuilder<Permissao> builder)
        {
            builder.ToTable("Permissoes");
            builder.Property(p => p.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(p => p.Descricao).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(p => p.Descricao).HasDatabaseName("IX_Permissoes_Descricao");
        }
    }
}
