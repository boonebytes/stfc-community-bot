using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "zone_services",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Description = table.Column<string>(nullable: true),
                    ZoneId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone_services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_zone_services_zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "zone_service_cost",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ResourceId = table.Column<int>(nullable: true),
                    Cost = table.Column<long>(nullable: false),
                    ServiceId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zone_service_cost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_zone_service_cost_ct_resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ct_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_zone_service_cost_zone_services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "zone_services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ct_resources",
                columns: new[] { "Id", "Label", "Name" },
                values: new object[,]
                {
                    { 51, "Refined Isogen Tier 1", "RefinedIsogenTier1" },
                    { 52, "Refined Isogen Tier 2", "RefinedIsogenTier2" },
                    { 53, "Refined Isogen Tier 3", "RefinedIsogenTier3" },
                    { 61, "Progenitor Emitters", "ProgenitorEmitters" },
                    { 62, "Progenitor Diodes", "ProgenitorDiodes" },
                    { 63, "Progenitor Cores", "ProgenitorCores" },
                    { 64, "ProgenitorReactors", "ProgenitorReactors" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_zone_service_cost_ResourceId",
                table: "zone_service_cost",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_zone_service_cost_ServiceId",
                table: "zone_service_cost",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_zone_services_ZoneId",
                table: "zone_services",
                column: "ZoneId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "zone_service_cost");

            migrationBuilder.DropTable(
                name: "zone_services");

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "ct_resources",
                keyColumn: "Id",
                keyValue: 64);
        }
    }
}
