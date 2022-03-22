using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DEFEND_EASTERN_TIME",
                table: "ZONES",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "INTERVAL DAY(8) TO SECOND(7)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DEFEND_EASTERN_TIME",
                table: "ZONES",
                type: "INTERVAL DAY(8) TO SECOND(7)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
