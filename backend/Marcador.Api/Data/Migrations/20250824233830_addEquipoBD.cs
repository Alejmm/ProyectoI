using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class addEquipoBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PartidosHistoricos_EquipoLocalId",
                table: "PartidosHistoricos",
                column: "EquipoLocalId");

            migrationBuilder.CreateIndex(
                name: "IX_PartidosHistoricos_EquipoVisitanteId",
                table: "PartidosHistoricos",
                column: "EquipoVisitanteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PartidosHistoricos_EquipoLocalId",
                table: "PartidosHistoricos");

            migrationBuilder.DropIndex(
                name: "IX_PartidosHistoricos_EquipoVisitanteId",
                table: "PartidosHistoricos");
        }
    }
}
