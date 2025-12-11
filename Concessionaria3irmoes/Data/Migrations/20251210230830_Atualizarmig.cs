using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concessionaria3irmoes.Data.Migrations
{
    /// <inheritdoc />
    public partial class Atualizarmig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Autonomia",
                table: "Veiculos");

            migrationBuilder.RenameColumn(
                name: "Torque",
                table: "Veiculos",
                newName: "Quilometragem");

            migrationBuilder.RenameColumn(
                name: "Capacidade",
                table: "Veiculos",
                newName: "Ano");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quilometragem",
                table: "Veiculos",
                newName: "Torque");

            migrationBuilder.RenameColumn(
                name: "Ano",
                table: "Veiculos",
                newName: "Capacidade");

            migrationBuilder.AddColumn<string>(
                name: "Autonomia",
                table: "Veiculos",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
