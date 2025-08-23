using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Faltas_EquipoId",
                table: "Faltas",
                column: "EquipoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Faltas_Equipos_EquipoId",
                table: "Faltas",
                column: "EquipoId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faltas_Equipos_EquipoId",
                table: "Faltas");

            migrationBuilder.DropIndex(
                name: "IX_Faltas_EquipoId",
                table: "Faltas");
        }
    }
}
