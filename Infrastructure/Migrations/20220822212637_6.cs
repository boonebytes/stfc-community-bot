using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DEFENDBROADCASTPINGFORLOWRISK",
                table: "ALLIANCES",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DEFENDBROADCASTPINGROLE",
                table: "ALLIANCES",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DEFENDBROADCASTPINGFORLOWRISK",
                table: "ALLIANCES");

            migrationBuilder.DropColumn(
                name: "DEFENDBROADCASTPINGROLE",
                table: "ALLIANCES");
        }
    }
}
