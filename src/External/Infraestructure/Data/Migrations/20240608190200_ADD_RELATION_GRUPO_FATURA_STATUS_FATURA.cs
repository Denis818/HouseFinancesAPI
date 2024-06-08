using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD_RELATION_GRUPO_FATURA_STATUS_FATURA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GrupoFaturaId",
                table: "Status_Faturas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Status_Faturas_GrupoFaturaId",
                table: "Status_Faturas",
                column: "GrupoFaturaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaId",
                table: "Status_Faturas",
                column: "GrupoFaturaId",
                principalTable: "Grupo_Fatura",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Status_Faturas_Grupo_Fatura_GrupoFaturaId",
                table: "Status_Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Status_Faturas_GrupoFaturaId",
                table: "Status_Faturas");

            migrationBuilder.DropColumn(
                name: "GrupoFaturaId",
                table: "Status_Faturas");
        }
    }
}
