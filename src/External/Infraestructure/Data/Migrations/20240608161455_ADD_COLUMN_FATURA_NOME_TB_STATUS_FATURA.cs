using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD_COLUMN_FATURA_NOME_TB_STATUS_FATURA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Status_Fatura_Id",
                table: "Status_Faturas");

            migrationBuilder.AddColumn<string>(
                name: "Fatura_Nome",
                table: "Status_Faturas",
                type: "varchar(20)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Fatura_Fatura_Nome",
                table: "Status_Faturas",
                column: "Fatura_Nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Status_Fatura_Fatura_Nome",
                table: "Status_Faturas");

            migrationBuilder.DropColumn(
                name: "Fatura_Nome",
                table: "Status_Faturas");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Fatura_Id",
                table: "Status_Faturas",
                column: "Id");
        }
    }
}
