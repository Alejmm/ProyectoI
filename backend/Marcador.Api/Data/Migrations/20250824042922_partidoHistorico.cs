using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class partidoHistorico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PartidosHistoricos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipoLocalId = table.Column<int>(type: "int", nullable: false),
                    EquipoVisitanteId = table.Column<int>(type: "int", nullable: false),
                    NombreLocal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreVisitante = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PuntosLocal = table.Column<int>(type: "int", nullable: false),
                    PuntosVisitante = table.Column<int>(type: "int", nullable: false),
                    FaltasLocal = table.Column<int>(type: "int", nullable: false),
                    FaltasVisitante = table.Column<int>(type: "int", nullable: false),
                    Cuarto = table.Column<int>(type: "int", nullable: false),
                    EnProrroga = table.Column<bool>(type: "bit", nullable: false),
                    NumeroProrroga = table.Column<int>(type: "int", nullable: false),
                    DuracionCuartoSeg = table.Column<int>(type: "int", nullable: false),
                    TiempoFinalSeg = table.Column<int>(type: "int", nullable: false),
                    MotivoFin = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartidosHistoricos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PartidosHistoricos");
        }
    }
}
