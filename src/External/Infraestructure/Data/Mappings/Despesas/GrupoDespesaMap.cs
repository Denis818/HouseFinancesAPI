using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Mappings.Despesas
{
    internal class GrupoDespesaMap : IEntityTypeConfiguration<GrupoDespesa>
    {
        public void Configure(EntityTypeBuilder<GrupoDespesa> builder)
        {
            builder.ToTable("Grupo_Despesa");
            builder.Property(c => c.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(c => c.Nome).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(c => c.Nome).HasDatabaseName("IX_Grupo_Despesa_Nome");
        }
    }
}
