using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_TB_GRUPO_DESPESAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoDespesaId",
                table: "Despesas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Grupo_Despesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(30)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grupo_Despesa", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_GrupoDespesaId",
                table: "Despesas",
                column: "GrupoDespesaId");

            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Despesa_Nome",
                table: "Grupo_Despesa",
                column: "Nome");

            migrationBuilder.AddForeignKey(
                name: "FK_Despesas_Grupo_Despesa_GrupoDespesaId",
                table: "Despesas",
                column: "GrupoDespesaId",
                principalTable: "Grupo_Despesa",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Despesas_Grupo_Despesa_GrupoDespesaId",
                table: "Despesas");

            migrationBuilder.DropTable(
                name: "Grupo_Despesa");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_GrupoDespesaId",
                table: "Despesas");

            migrationBuilder.DropColumn(
                name: "GrupoDespesaId",
                table: "Despesas");
        }
    }
}
