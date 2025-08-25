using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPartidoHistoricoNuevo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaUtc",
                table: "PartidosHistoricos",
                newName: "Fecha");

            migrationBuilder.AlterColumn<string>(
                name: "NombreVisitante",
                table: "PartidosHistoricos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NombreLocal",
                table: "PartidosHistoricos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "EquipoVisitanteId",
                table: "PartidosHistoricos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "EquipoLocalId",
                table: "PartidosHistoricos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "PartidosHistoricos",
                newName: "FechaUtc");

            migrationBuilder.AlterColumn<string>(
                name: "NombreVisitante",
                table: "PartidosHistoricos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NombreLocal",
                table: "PartidosHistoricos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EquipoVisitanteId",
                table: "PartidosHistoricos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EquipoLocalId",
                table: "PartidosHistoricos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
