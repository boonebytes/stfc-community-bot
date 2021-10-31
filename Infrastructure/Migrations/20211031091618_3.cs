using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelationshipId",
                table: "alliance_diplomacy",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_alliance_diplomacy_RelationshipId",
                table: "alliance_diplomacy",
                column: "RelationshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_alliance_diplomacy_ct_diplomatic_relation_RelationshipId",
                table: "alliance_diplomacy",
                column: "RelationshipId",
                principalTable: "ct_diplomatic_relation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_alliance_diplomacy_ct_diplomatic_relation_RelationshipId",
                table: "alliance_diplomacy");

            migrationBuilder.DropIndex(
                name: "IX_alliance_diplomacy_RelationshipId",
                table: "alliance_diplomacy");

            migrationBuilder.DropColumn(
                name: "RelationshipId",
                table: "alliance_diplomacy");
        }
    }
}
