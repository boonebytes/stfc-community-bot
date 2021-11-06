using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefendEasternDay",
                table: "zones",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DefendEasternTime",
                table: "zones",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefendEasternDay",
                table: "zones");

            migrationBuilder.DropColumn(
                name: "DefendEasternTime",
                table: "zones");
        }
    }
}
