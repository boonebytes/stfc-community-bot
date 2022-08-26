using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CT_DIPLOMATIC_RELATION",
                keyColumn: "ID",
                keyValue: -1,
                column: "NAME",
                value: "Untrusted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CT_DIPLOMATIC_RELATION",
                keyColumn: "ID",
                keyValue: -1,
                column: "NAME",
                value: "Pew");
        }
    }
}
