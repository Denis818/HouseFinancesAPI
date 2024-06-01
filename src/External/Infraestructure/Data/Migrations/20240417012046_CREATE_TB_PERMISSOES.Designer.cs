﻿// <auto-generated />
using System;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations
{
    [DbContext(typeof(FinanceDbContext))]
    [Migration("20240417012046_CREATE_TB_PERMISSOES")]
    partial class CREATE_TB_PERMISSOES
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Domain.Models.Finance.Categoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Descricao")
                        .HasDatabaseName("IX_Categorias_Descricao");

                    b.ToTable("Categorias", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Finance.Despesa", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoriaId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DataCompra")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Fornecedor")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Item")
                        .IsRequired()
                        .HasColumnType("varchar(40)");

                    b.Property<double>("Preco")
                        .HasColumnType("double(7, 2)");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<double>("Total")
                        .HasColumnType("double(7, 2)");

                    b.HasKey("Id");

                    b.HasIndex("DataCompra")
                        .HasDatabaseName("IX_Despesas_DataCompra");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Despesas_Id");

                    b.HasIndex("Item")
                        .HasDatabaseName("IX_Despesas_Item");

                    b.ToTable("Despesas", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Finance.Membro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Nome")
                        .HasDatabaseName("IX_Membros_Nome");

                    b.ToTable("Membros", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Users.Permissao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Descricao")
                        .HasDatabaseName("IX_Permissoes_Descricao");

                    b.ToTable("Permissoes", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Finance.Despesa", b =>
                {
                    b.HasOne("Domain.Models.Finance.Categoria", "Categoria")
                        .WithMany("Despesas")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");
                });

            modelBuilder.Entity("Domain.Models.Finance.Categoria", b =>
                {
                    b.Navigation("Despesas");
                });
#pragma warning restore 612, 618
        }
    }
}
