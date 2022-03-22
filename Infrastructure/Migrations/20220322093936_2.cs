using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ZONES",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONES",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ZONE_SERVICES",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICES",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ZONE_SERVICE_COST",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICE_COST",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ZONE_NEIGHBOURS",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_NEIGHBOURS",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "STARSYSTEMS",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "STARSYSTEMS",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "DIRECT_MESSAGES",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "DIRECT_MESSAGES",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "AUDIT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "AUDIT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ALLIANCES",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCES",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ALLIANCE_GROUPS",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_GROUPS",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MODIFIED_BY",
                table: "ALLIANCE_DIPLOMACY",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_DIPLOMACY",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ZONES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ZONES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ZONE_SERVICES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ZONE_SERVICE_COST");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICE_COST");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ZONE_NEIGHBOURS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ZONE_NEIGHBOURS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "STARSYSTEMS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "STARSYSTEMS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "DIRECT_MESSAGES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "DIRECT_MESSAGES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "AUDIT");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "AUDIT");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ALLIANCES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ALLIANCES");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ALLIANCE_GROUPS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_GROUPS");

            migrationBuilder.DropColumn(
                name: "MODIFIED_BY",
                table: "ALLIANCE_DIPLOMACY");

            migrationBuilder.DropColumn(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_DIPLOMACY");
        }
    }
}
