using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FCG.Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "bibliotecas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bibliotecas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "jogos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "varchar(120)", nullable: false),
                    descricao = table.Column<string>(type: "varchar(1000)", nullable: false),
                    preco = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    categoria = table.Column<string>(type: "varchar(100)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jogos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "pedidos",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    price = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pedidos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "itens_biblioteca",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    jogo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dt_aquisicao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BibliotecaId = table.Column<Guid>(type: "uuid", nullable: false),
                    dt_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    dt_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_itens_biblioteca", x => x.id);
                    table.ForeignKey(
                        name: "FK_itens_biblioteca_bibliotecas_BibliotecaId",
                        column: x => x.BibliotecaId,
                        principalTable: "bibliotecas",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_itens_biblioteca_jogos_jogo_id",
                        column: x => x.jogo_id,
                        principalTable: "jogos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bibliotecas_usuario_id",
                table: "bibliotecas",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itens_biblioteca_BibliotecaId_jogo_id",
                table: "itens_biblioteca",
                columns: new[] { "BibliotecaId", "jogo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_itens_biblioteca_jogo_id",
                table: "itens_biblioteca",
                column: "jogo_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itens_biblioteca");

            migrationBuilder.DropTable(
                name: "pedidos");

            migrationBuilder.DropTable(
                name: "bibliotecas");

            migrationBuilder.DropTable(
                name: "jogos");
        }
    }
}
