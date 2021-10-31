using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AllianceGroupId",
                table: "alliances",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "alliance_groups",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alliance_groups", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alliances_AllianceGroupId",
                table: "alliances",
                column: "AllianceGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_alliances_alliance_groups_AllianceGroupId",
                table: "alliances",
                column: "AllianceGroupId",
                principalTable: "alliance_groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alliances_alliance_groups_AllianceGroupId",
                table: "alliances");

            migrationBuilder.DropTable(
                name: "alliance_groups");

            migrationBuilder.DropIndex(
                name: "IX_alliances_AllianceGroupId",
                table: "alliances");

            migrationBuilder.DropColumn(
                name: "AllianceGroupId",
                table: "alliances");
        }
    }
}
