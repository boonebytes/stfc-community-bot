using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CT_ALLIANCE_SERVICE_LEVEL",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false, defaultValue: 0),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    LABEL = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_ALLIANCE_SERVICE_LEVEL", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ALLIANCE_SERVICES",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MODIFIED_BY = table.Column<string>(maxLength: 500, nullable: true),
                    MODIFIED_DATE = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSDATE"),
                    ALLIANCE_ID = table.Column<long>(nullable: false),
                    ZONE_SERVICE_ID = table.Column<long>(nullable: false),
                    LEVEL_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLIANCE_SERVICES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_SERVICES_ALLIANCES_ALLIANCE_ID",
                        column: x => x.ALLIANCE_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_SERVICES_CT_ALLIANCE_SERVICE_LEVEL_LEVEL_ID",
                        column: x => x.LEVEL_ID,
                        principalTable: "CT_ALLIANCE_SERVICE_LEVEL",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_SERVICES_ZONE_SERVICES_ZONE_SERVICE_ID",
                        column: x => x.ZONE_SERVICE_ID,
                        principalTable: "ZONE_SERVICES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 0, "Unspecified", "Unspecified" },
                    { 62, "Progenitor Diodes", "ProgenitorDiodes" },
                    { 61, "Progenitor Emitters", "ProgenitorEmitters" },
                    { 53, "Refined Isogen Tier 3", "RefinedIsogenTier3" },
                    { 52, "Refined Isogen Tier 2", "RefinedIsogenTier2" },
                    { 51, "Refined Isogen Tier 1", "RefinedIsogenTier1" },
                    { 44, "Ore Tier 4", "OreTier4" },
                    { 43, "Ore Tier 3", "OreTier3" },
                    { 34, "Crystal Tier 4", "CrystalTier4" },
                    { 33, "Crystal Tier 3", "CrystalTier3" },
                    { 24, "Gas Tier 4", "GasTier4" },
                    { 23, "Gas Tier 3", "GasTier3" },
                    { 13, "Isogen Tier 3", "IsogenTier3" },
                    { 12, "Isogen Tier 2", "IsogenTier2" },
                    { 11, "Isogen Tier 1", "IsogenTier1" },
                    { 3, "Dilithium", "Dilithium" },
                    { 2, "Tritanium", "Tritanium" },
                    { 1, "Parasteel", "Parasteel" },
                    { 63, "Progenitor Cores", "ProgenitorCores" },
                    { 64, "ProgenitorReactors", "ProgenitorReactors" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_SERVICES_ALLIANCE_ID",
                table: "ALLIANCE_SERVICES",
                column: "ALLIANCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_SERVICES_LEVEL_ID",
                table: "ALLIANCE_SERVICES",
                column: "LEVEL_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_SERVICES_ZONE_SERVICE_ID",
                table: "ALLIANCE_SERVICES",
                column: "ZONE_SERVICE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALLIANCE_SERVICES");

            migrationBuilder.DropTable(
                name: "CT_ALLIANCE_SERVICE_LEVEL");
        }
    }
}
