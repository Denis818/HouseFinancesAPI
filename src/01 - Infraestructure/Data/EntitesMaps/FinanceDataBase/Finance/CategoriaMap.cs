using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.EntitesMaps.FinanceDataBase.Finance
{
    internal class CategoriaMap : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("Categorias");
            builder.Property(c => c.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(c => c.Descricao).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(c => c.Descricao).HasDatabaseName("IX_Categorias_Descricao");
        }
    }
}
