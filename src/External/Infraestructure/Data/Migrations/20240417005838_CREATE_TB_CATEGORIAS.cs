using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_TB_CATEGORIAS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder
                .CreateTable(
                    name: "Categorias",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Descricao = table
                            .Column<string>(type: "varchar(50)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Categorias", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Descricao",
                table: "Categorias",
                column: "Descricao"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Id",
                table: "Categorias",
                column: "Id"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Categorias");
        }
    }
}
