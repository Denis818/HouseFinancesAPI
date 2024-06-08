using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TB_STATUS_FATURAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Status_Faturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Estado = table.Column<string>(type: "varchar(30)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status_Faturas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_GrupoFaturaId",
                table: "Despesas",
                column: "GrupoFaturaId");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Fatura_Estado",
                table: "Status_Faturas",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_Status_Fatura_Id",
                table: "Status_Faturas",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Status_Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas");

            migrationBuilder.DropIndex(
                name: "IX_Despesas_GrupoFaturaId",
                table: "Despesas");
        }
    }
}
