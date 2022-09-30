using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CT_RESOURCES",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 71, "Collisional Plasma", "CollisionalPlasma" },
                    { 72, "Magnetic Plasma", "MagneticPlasma" },
                    { 73, "Subspace Superconductor", "SubspaceSuperconductor" },
                    { 74, "Alliance Reserves", "AllianceReserves" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 74);
        }
    }
}
