using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "THREATS",
                table: "ZONES");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "THREATS",
                table: "ZONES",
                type: "NVARCHAR2(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
