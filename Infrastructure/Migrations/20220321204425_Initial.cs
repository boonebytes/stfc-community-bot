using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ALLIANCE_GROUPS",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLIANCE_GROUPS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<string>(maxLength: 200, nullable: true),
                    TYPE = table.Column<string>(maxLength: 50, nullable: true),
                    TABLE_NAME = table.Column<string>(maxLength: 200, nullable: true),
                    DATE_TIME = table.Column<DateTime>(nullable: false),
                    OLD_VALUES = table.Column<string>(nullable: true),
                    NEW_VALUES = table.Column<string>(nullable: true),
                    AFFECTED_COLUMNS = table.Column<string>(nullable: true),
                    PRIMARY_KEY = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CT_DIPLOMATIC_RELATION",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false, defaultValue: 0),
                    NAME = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DIPLOMATIC_RELATION", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CT_RESOURCES",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false, defaultValue: 0),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    LABEL = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_RESOURCES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DIRECT_MESSAGES",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RECEIVED_TIMESTAMP = table.Column<DateTime>(nullable: false),
                    FROM_USER = table.Column<decimal>(nullable: false),
                    COMMON_SERVERS = table.Column<string>(maxLength: 4000, nullable: true),
                    MESSAGE = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DIRECT_MESSAGES", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ALLIANCES",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(maxLength: 2000, nullable: true),
                    ACRONYM = table.Column<string>(maxLength: 10, nullable: true),
                    ALLIANCE_GROUP_ID = table.Column<long>(nullable: true),
                    GUILD_ID = table.Column<decimal>(nullable: true),
                    DEFEND_SCHEDULE_POST_CHANNEL = table.Column<decimal>(nullable: true),
                    DEFEND_SCHEDULE_POST_TIME = table.Column<string>(maxLength: 10, nullable: true),
                    DEFEND_BROADCAST_LEAD_TIME = table.Column<int>(nullable: true),
                    NEXT_SCHEDULED_POST = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLIANCES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALLIANCES_ALLIANCE_GROUPS_ALLIANCE_GROUP_ID",
                        column: x => x.ALLIANCE_GROUP_ID,
                        principalTable: "ALLIANCE_GROUPS",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ALLIANCE_DIPLOMACY",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    OWNER_ID = table.Column<long>(nullable: false),
                    RELATED_ID = table.Column<long>(nullable: false),
                    RELATIONSHIP_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ALLIANCE_DIPLOMACY", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_DIPLOMACY_ALLIANCES_OWNER_ID",
                        column: x => x.OWNER_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_DIPLOMACY_ALLIANCES_RELATED_ID",
                        column: x => x.RELATED_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ALLIANCE_DIPLOMACY_CT_DIPLOMATIC_RELATION_RELATIONSHIP_ID",
                        column: x => x.RELATIONSHIP_ID,
                        principalTable: "CT_DIPLOMATIC_RELATION",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZONES",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    LEVEL = table.Column<int>(nullable: false),
                    THREATS = table.Column<string>(maxLength: 2000, nullable: true),
                    DEFEND_DAY_OF_WEEK = table.Column<string>(maxLength: 15, nullable: false),
                    DEFEND_UTC_TIME = table.Column<string>(maxLength: 10, nullable: false),
                    DEFEND_EASTERN_DAY = table.Column<int>(nullable: true),
                    DEFEND_EASTERN_TIME = table.Column<TimeSpan>(nullable: true),
                    NOTES = table.Column<string>(nullable: true),
                    OWNER_ID = table.Column<long>(nullable: true),
                    NEXT_DEFEND = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZONES_ALLIANCES_OWNER_ID",
                        column: x => x.OWNER_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "STARSYSTEMS",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    ZONE_ID = table.Column<long>(nullable: false),
                    RESOURCE_ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_STARSYSTEMS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_STARSYSTEMS_CT_RESOURCES_RESOURCE_ID",
                        column: x => x.RESOURCE_ID,
                        principalTable: "CT_RESOURCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_STARSYSTEMS_ZONES_ZONE_ID",
                        column: x => x.ZONE_ID,
                        principalTable: "ZONES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZONE_NEIGHBOURS",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    FROM_ZONE_ID = table.Column<long>(nullable: false),
                    TO_ZONE_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONE_NEIGHBOURS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZONE_NEIGHBOURS_ZONES_FROM_ZONE_ID",
                        column: x => x.FROM_ZONE_ID,
                        principalTable: "ZONES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZONE_NEIGHBOURS_ZONES_TO_ZONE_ID",
                        column: x => x.TO_ZONE_ID,
                        principalTable: "ZONES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZONE_SERVICES",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    DESCRIPTION = table.Column<string>(nullable: true),
                    ZONE_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONE_SERVICES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZONE_SERVICES_ZONES_ZONE_ID",
                        column: x => x.ZONE_ID,
                        principalTable: "ZONES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZONE_SERVICE_COST",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    RESOURCE_ID = table.Column<int>(nullable: false),
                    COST = table.Column<long>(nullable: false),
                    SERVICE_ID = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZONE_SERVICE_COST", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ZONE_SERVICE_COST_CT_RESOURCES_RESOURCE_ID",
                        column: x => x.RESOURCE_ID,
                        principalTable: "CT_RESOURCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZONE_SERVICE_COST_ZONE_SERVICES_SERVICE_ID",
                        column: x => x.SERVICE_ID,
                        principalTable: "ZONE_SERVICES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CT_DIPLOMATIC_RELATION",
                columns: new[] { "ID", "NAME" },
                values: new object[,]
                {
                    { 0, "Unspecified" },
                    { -1, "Enemy" },
                    { 1, "Neutral" },
                    { 2, "Friendly" },
                    { 3, "Allied" }
                });

            migrationBuilder.InsertData(
                table: "CT_RESOURCES",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 62, "Progenitor Diodes", "ProgenitorDiodes" },
                    { 61, "Progenitor Emitters", "ProgenitorEmitters" },
                    { 53, "Refined Isogen Tier 3", "RefinedIsogenTier3" },
                    { 52, "Refined Isogen Tier 2", "RefinedIsogenTier2" },
                    { 51, "Refined Isogen Tier 1", "RefinedIsogenTier1" },
                    { 44, "Ore Tier 4", "OreTier4" },
                    { 43, "Ore Tier 3", "OreTier3" },
                    { 34, "Crystal Tier 4", "CrystalTier4" },
                    { 33, "Crystal Tier 3", "CrystalTier3" },
                    { 23, "Gas Tier 3", "GasTier3" },
                    { 63, "Progenitor Cores", "ProgenitorCores" },
                    { 13, "Isogen Tier 3", "IsogenTier3" },
                    { 12, "Isogen Tier 2", "IsogenTier2" },
                    { 11, "Isogen Tier 1", "IsogenTier1" },
                    { 3, "Dilithium", "Dilithium" },
                    { 2, "Tritanium", "Tritanium" },
                    { 1, "Parasteel", "Parasteel" },
                    { 0, "Unspecified", "Unspecified" },
                    { 24, "Gas Tier 4", "GasTier4" },
                    { 64, "ProgenitorReactors", "ProgenitorReactors" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_DIPLOMACY_OWNER_ID",
                table: "ALLIANCE_DIPLOMACY",
                column: "OWNER_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_DIPLOMACY_RELATED_ID",
                table: "ALLIANCE_DIPLOMACY",
                column: "RELATED_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCE_DIPLOMACY_RELATIONSHIP_ID",
                table: "ALLIANCE_DIPLOMACY",
                column: "RELATIONSHIP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ALLIANCES_ALLIANCE_GROUP_ID",
                table: "ALLIANCES",
                column: "ALLIANCE_GROUP_ID");

            migrationBuilder.CreateIndex(
                name: "IX_STARSYSTEMS_RESOURCE_ID",
                table: "STARSYSTEMS",
                column: "RESOURCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_STARSYSTEMS_ZONE_ID",
                table: "STARSYSTEMS",
                column: "ZONE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONE_NEIGHBOURS_FROM_ZONE_ID",
                table: "ZONE_NEIGHBOURS",
                column: "FROM_ZONE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONE_NEIGHBOURS_TO_ZONE_ID",
                table: "ZONE_NEIGHBOURS",
                column: "TO_ZONE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONE_SERVICE_COST_RESOURCE_ID",
                table: "ZONE_SERVICE_COST",
                column: "RESOURCE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONE_SERVICE_COST_SERVICE_ID",
                table: "ZONE_SERVICE_COST",
                column: "SERVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONE_SERVICES_ZONE_ID",
                table: "ZONE_SERVICES",
                column: "ZONE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_ZONES_OWNER_ID",
                table: "ZONES",
                column: "OWNER_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ALLIANCE_DIPLOMACY");

            migrationBuilder.DropTable(
                name: "AUDIT");

            migrationBuilder.DropTable(
                name: "DIRECT_MESSAGES");

            migrationBuilder.DropTable(
                name: "STARSYSTEMS");

            migrationBuilder.DropTable(
                name: "ZONE_NEIGHBOURS");

            migrationBuilder.DropTable(
                name: "ZONE_SERVICE_COST");

            migrationBuilder.DropTable(
                name: "CT_DIPLOMATIC_RELATION");

            migrationBuilder.DropTable(
                name: "CT_RESOURCES");

            migrationBuilder.DropTable(
                name: "ZONE_SERVICES");

            migrationBuilder.DropTable(
                name: "ZONES");

            migrationBuilder.DropTable(
                name: "ALLIANCES");

            migrationBuilder.DropTable(
                name: "ALLIANCE_GROUPS");
        }
    }
}
