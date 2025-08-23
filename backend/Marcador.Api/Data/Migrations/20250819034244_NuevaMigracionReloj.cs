using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Marcador.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class NuevaMigracionReloj : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RelojCorriendo",
                table: "Marcadores",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RelojCorriendo",
                table: "Marcadores");
        }
    }
}
