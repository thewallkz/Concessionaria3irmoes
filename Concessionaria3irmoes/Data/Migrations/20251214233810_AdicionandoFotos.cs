using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concessionaria3irmoes.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionandoFotos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VeiculoFoto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CaminhoArquivo = table.Column<string>(type: "TEXT", nullable: false),
                    VeiculoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VeiculoFoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VeiculoFoto_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VeiculoFoto_VeiculoId",
                table: "VeiculoFoto",
                column: "VeiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VeiculoFoto");
        }
    }
}
