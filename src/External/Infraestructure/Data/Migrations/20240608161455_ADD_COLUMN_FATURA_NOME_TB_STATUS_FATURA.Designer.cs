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
    [Migration("20240608161455_ADD_COLUMN_FATURA_NOME_TB_STATUS_FATURA")]
    partial class ADD_COLUMN_FATURA_NOME_TB_STATUS_FATURA
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Domain.Models.Categorias.Categoria", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descricao")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Descricao")
                        .HasDatabaseName("IX_Categorias_Descricao");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Categorias_Id");

                    b.ToTable("Categorias", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Despesas.Despesa", b =>
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
                        .HasColumnType("varchar(50)");

                    b.Property<int>("GrupoFaturaId")
                        .HasColumnType("int");

                    b.Property<string>("Item")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<double>("Preco")
                        .HasColumnType("double(7, 2)");

                    b.Property<int>("Quantidade")
                        .HasColumnType("int");

                    b.Property<double>("Total")
                        .HasColumnType("double(7, 2)");

                    b.HasKey("Id");

                    b.HasIndex("CategoriaId");

                    b.HasIndex("DataCompra")
                        .HasDatabaseName("IX_Despesas_DataCompra");

                    b.HasIndex("Fornecedor")
                        .HasDatabaseName("IX_Despesas_Fornecedor");

                    b.HasIndex("GrupoFaturaId");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Despesas_Id");

                    b.HasIndex("Item")
                        .HasDatabaseName("IX_Despesas_Item");

                    b.ToTable("Despesas", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Despesas.GrupoFatura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Grupo_Fatura_Id");

                    b.HasIndex("Nome")
                        .HasDatabaseName("IX_Grupo_Fatura_Nome");

                    b.ToTable("Grupo_Fatura", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Despesas.StatusFatura", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Estado")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("FaturaNome")
                        .IsRequired()
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Fatura_Nome");

                    b.HasKey("Id");

                    b.HasIndex("Estado")
                        .HasDatabaseName("IX_Status_Fatura_Estado");

                    b.HasIndex("FaturaNome")
                        .HasDatabaseName("IX_Status_Fatura_Fatura_Nome");

                    b.ToTable("Status_Faturas", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Membros.Membro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("Telefone");

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

            modelBuilder.Entity("Domain.Models.Users.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(30)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .HasDatabaseName("IX_Email");

                    b.HasIndex("Id")
                        .HasDatabaseName("IX_Usuarios_Id");

                    b.ToTable("Usuarios", (string)null);
                });

            modelBuilder.Entity("Usuario_Permissao", b =>
                {
                    b.Property<int>("Usuario_Id")
                        .HasColumnType("int");

                    b.Property<int>("Permissao_Id")
                        .HasColumnType("int");

                    b.HasKey("Usuario_Id", "Permissao_Id");

                    b.HasIndex("Permissao_Id");

                    b.ToTable("Usuario_Permissao", (string)null);
                });

            modelBuilder.Entity("Domain.Models.Despesas.Despesa", b =>
                {
                    b.HasOne("Domain.Models.Categorias.Categoria", "Categoria")
                        .WithMany("Despesas")
                        .HasForeignKey("CategoriaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Models.Despesas.GrupoFatura", "GrupoFatura")
                        .WithMany("Despesas")
                        .HasForeignKey("GrupoFaturaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Categoria");

                    b.Navigation("GrupoFatura");
                });

            modelBuilder.Entity("Usuario_Permissao", b =>
                {
                    b.HasOne("Domain.Models.Users.Permissao", null)
                        .WithMany()
                        .HasForeignKey("Permissao_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Models.Users.Usuario", null)
                        .WithMany()
                        .HasForeignKey("Usuario_Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Models.Categorias.Categoria", b =>
                {
                    b.Navigation("Despesas");
                });

            modelBuilder.Entity("Domain.Models.Despesas.GrupoFatura", b =>
                {
                    b.Navigation("Despesas");
                });
#pragma warning restore 612, 618
        }
    }
}
