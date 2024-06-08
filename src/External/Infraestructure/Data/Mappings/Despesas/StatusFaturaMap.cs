using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class StatusFaturaMap : IEntityTypeConfiguration<StatusFatura>
    {
        public void Configure(EntityTypeBuilder<StatusFatura> builder)
        {
            builder.ToTable("Status_Faturas");
            builder.Property(e => e.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();
            builder.Property(e => e.Estado).HasColumnType("varchar(30)").IsRequired();
            builder.Property(e => e.FaturaNome).HasColumnName("Fatura_Nome").HasColumnType("varchar(20)").IsRequired();

            builder.HasIndex(c => c.FaturaNome).HasDatabaseName("IX_Status_Fatura_Fatura_Nome");
            builder.HasIndex(c => c.Estado).HasDatabaseName("IX_Status_Fatura_Estado");
        }
    }
}
