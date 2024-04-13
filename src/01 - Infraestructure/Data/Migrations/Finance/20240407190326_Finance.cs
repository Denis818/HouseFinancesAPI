using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations.Finance
{
    /// <inheritdoc />
    public partial class Finance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

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
                            .Column<string>(type: "varchar(30)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Categorias", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Membros",
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
                        table.PrimaryKey("PK_Membros", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Permissoes",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Descricao = table
                            .Column<string>(type: "varchar(30)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Permissoes", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Usuarios",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Email = table
                            .Column<string>(type: "varchar(30)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Password = table
                            .Column<string>(type: "varchar(100)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Salt = table
                            .Column<string>(type: "varchar(100)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Usuarios", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Despesas",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        DataCompra = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        Item = table
                            .Column<string>(type: "varchar(40)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Preco = table.Column<double>(type: "double(6,2)", nullable: false),
                        Quantidade = table.Column<int>(type: "int", nullable: false),
                        Fornecedor = table
                            .Column<string>(type: "varchar(20)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Total = table.Column<double>(type: "double(6,2)", nullable: false),
                        CategoriaId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Despesas", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Despesas_Categorias_CategoriaId",
                            column: x => x.CategoriaId,
                            principalTable: "Categorias",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "PermissaoUsuario",
                    columns: table => new
                    {
                        PermissoesId = table.Column<int>(type: "int", nullable: false),
                        UsuariosId = table.Column<int>(type: "int", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey(
                            "PK_PermissaoUsuario",
                            x => new { x.PermissoesId, x.UsuariosId }
                        );
                        table.ForeignKey(
                            name: "FK_PermissaoUsuario_Permissoes_PermissoesId",
                            column: x => x.PermissoesId,
                            principalTable: "Permissoes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                        table.ForeignKey(
                            name: "FK_PermissaoUsuario_Usuarios_UsuariosId",
                            column: x => x.UsuariosId,
                            principalTable: "Usuarios",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_Descricao",
                table: "Categorias",
                column: "Descricao"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_CategoriaId",
                table: "Despesas",
                column: "CategoriaId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_DataCompra",
                table: "Despesas",
                column: "DataCompra"
            );

            migrationBuilder.CreateIndex(name: "IX_Despesas_Id", table: "Despesas", column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Despesas_Item",
                table: "Despesas",
                column: "Item"
            );

            migrationBuilder.CreateIndex(name: "IX_Membros_Nome", table: "Membros", column: "Nome");

            migrationBuilder.CreateIndex(
                name: "IX_PermissaoUsuario_UsuariosId",
                table: "PermissaoUsuario",
                column: "UsuariosId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Permissoes_Descricao",
                table: "Permissoes",
                column: "Descricao"
            );

            migrationBuilder.CreateIndex(name: "IX_Email", table: "Usuarios", column: "Email");

            migrationBuilder.CreateIndex(name: "IX_Usuarios_Id", table: "Usuarios", column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Despesas");

            migrationBuilder.DropTable(name: "Membros");

            migrationBuilder.DropTable(name: "PermissaoUsuario");

            migrationBuilder.DropTable(name: "Categorias");

            migrationBuilder.DropTable(name: "Permissoes");

            migrationBuilder.DropTable(name: "Usuarios");
        }
    }
}
