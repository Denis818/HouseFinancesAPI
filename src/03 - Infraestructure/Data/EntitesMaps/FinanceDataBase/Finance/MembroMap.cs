using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.EntitesMaps.FinanceDataBase.Finance
{
    internal class MembroMap : IEntityTypeConfiguration<Membro>
    {
        public void Configure(EntityTypeBuilder<Membro> builder)
        {
            builder.ToTable("Membros");
            builder.Property(m => m.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(m => m.Nome).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(m => m.Nome).HasDatabaseName("IX_Membros_Nome");
        }
    }
}
