using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NextScheduledPost",
                table: "alliances",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextScheduledPost",
                table: "alliances");
        }
    }
}
