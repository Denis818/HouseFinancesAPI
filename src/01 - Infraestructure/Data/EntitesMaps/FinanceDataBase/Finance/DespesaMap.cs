using Domain.Models.Finance;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Data.EntitesMaps.FinanceDataBase.Finance
{
    internal class DespesaMap : IEntityTypeConfiguration<Despesa>
    {
        public void Configure(EntityTypeBuilder<Despesa> builder)
        {
            builder.ToTable("Despesas");
            builder.Property(d => d.Id).HasColumnType("int").IsRequired().ValueGeneratedOnAdd();

            builder.Property(d => d.DataCompra).HasColumnType("datetime(6)").IsRequired();
            builder.Property(d => d.Item).HasColumnType("varchar(40)").IsRequired();
            builder.Property(d => d.Preco).HasColumnType("decimal(6, 2)").IsRequired();
            builder.Property(d => d.Quantidade).HasColumnType("int").IsRequired();
            builder.Property(d => d.Fornecedor).HasColumnType("varchar(20)").IsRequired();
            builder.Property(d => d.Total).HasColumnType("decimal(6, 2)").IsRequired();
            builder.Property(d => d.CategoriaId).HasColumnType("int").IsRequired();

            builder.HasOne(d => d.Categoria)
                   .WithMany(c => c.Despesas)
                   .HasForeignKey(d => d.CategoriaId);

            builder.HasIndex(d => d.Id)
                   .HasDatabaseName("IX_Despesas_Id");

            builder.HasIndex(d => d.Item)
                   .HasDatabaseName("IX_Despesas_Item");

            builder.HasIndex(d => d.CategoriaId)
                   .HasDatabaseName("IX_Despesas_CategoriaId");

            builder.HasIndex(d => d.DataCompra)
                   .HasDatabaseName("IX_Despesas_DataCompra");
        }
    }
}
