﻿// <auto-generated />
using System;
using Data.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Data.Migrations.LogDb
{
    [DbContext(typeof(LogDbContext))]
    [Migration("20240326003904_LogsApp")]
    partial class LogsApp
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Domain.Models.LogApplication", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Content")
                        .HasColumnType("longtext");

                    b.Property<string>("ExceptionMessage")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("InclusionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Method")
                        .HasColumnType("longtext");

                    b.Property<string>("Path")
                        .HasColumnType("longtext");

                    b.Property<string>("QueryString")
                        .HasColumnType("longtext");

                    b.Property<string>("StackTrace")
                        .HasColumnType("longtext");

                    b.Property<string>("TypeLog")
                        .HasColumnType("longtext");

                    b.Property<string>("UserName")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("LogsApplication");
                });
#pragma warning restore 612, 618
        }
    }
}
