using Domain.Models.Categorias;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings.Finance
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
