using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_TB_Grupo_FaturaS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoFaturaId",
                table: "Despesas",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder
                .CreateTable(
                    name: "Grupo_Fatura",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Nome = table
                            .Column<string>(type: "varchar(30)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Grupo_Fatura", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Fatura_Id",
                table: "Grupo_Fatura",
                column: "Id"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Fatura_Nome",
                table: "Grupo_Fatura",
                column: "Nome"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaId",
                table: "Despesas",
                column: "GrupoFaturaId",
                principalTable: "Grupo_Fatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Grupo_Fatura_GrupoFaturaId",
                table: "Despesas"
            );

            migrationBuilder.DropTable(name: "Grupo_Fatura");

            migrationBuilder.DropColumn(name: "GrupoFaturaId", table: "Despesas");
        }
    }
}
