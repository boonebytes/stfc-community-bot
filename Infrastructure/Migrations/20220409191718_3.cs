using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONES",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICES",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICE_COST",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_NEIGHBOURS",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "STARSYSTEMS",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "DIRECT_MESSAGES",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "AUDIT",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCES",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AddColumn<decimal>(
                name: "ALLIED_BROADCAST_ROLE",
                table: "ALLIANCES",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_GROUPS",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_DIPLOMACY",
                nullable: false,
                defaultValueSql: "SYSDATE",
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ALLIED_BROADCAST_ROLE",
                table: "ALLIANCES");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONES",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICES",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_SERVICE_COST",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ZONE_NEIGHBOURS",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "STARSYSTEMS",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "DIRECT_MESSAGES",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "AUDIT",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCES",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_GROUPS",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MODIFIED_DATE",
                table: "ALLIANCE_DIPLOMACY",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "SYSDATE");
        }
    }
}
