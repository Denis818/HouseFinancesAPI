﻿using Domain.Models.Despesas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Mappings.Despesas
{
    internal class GrupoFaturaMap : IEntityTypeConfiguration<GrupoFatura>
    {
        public void Configure(EntityTypeBuilder<GrupoFatura> builder)
        {
            builder.ToTable("Grupo_Fatura");
            builder.Property(c => c.Id).IsRequired().HasColumnType("int").ValueGeneratedOnAdd();

            builder.Property(c => c.Nome).HasColumnType("varchar(30)").IsRequired();

            builder.HasIndex(c => c.Nome).HasDatabaseName("IX_Grupo_Fatura_Nome");
            builder.HasIndex(c => c.Id).HasDatabaseName("IX_Grupo_Fatura_Id");
        }
    }
}
