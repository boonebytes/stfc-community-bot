using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 73,
                column: "LABEL",
                value: "Subspace Superconductors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 73,
                column: "LABEL",
                value: "Subspace Superconductor");
        }
    }
}
