using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "zone_neighbours",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FromZoneId = table.Column<long>(nullable: false),
                    ToZoneId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone_neighbours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_zone_neighbours_zones_FromZoneId",
                        column: x => x.FromZoneId,
                        principalTable: "zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_zone_neighbours_zones_ToZoneId",
                        column: x => x.ToZoneId,
                        principalTable: "zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_zone_neighbours_FromZoneId",
                table: "zone_neighbours",
                column: "FromZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_zone_neighbours_ToZoneId",
                table: "zone_neighbours",
                column: "ToZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "zone_neighbours");
        }
    }
}
