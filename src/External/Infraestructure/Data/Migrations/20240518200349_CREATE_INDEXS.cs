using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_INDEXS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Grupo_Despesa_Id",
                table: "Grupo_Despesa",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Id",
                table: "Categorias",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Grupo_Despesa_Id",
                table: "Grupo_Despesa");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_Id",
                table: "Categorias");
        }
    }
}
