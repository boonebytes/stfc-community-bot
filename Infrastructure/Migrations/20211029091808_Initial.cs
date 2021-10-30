using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "alliances",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Acronym = table.Column<string>(maxLength: 5, nullable: true),
                    GuildId = table.Column<ulong>(nullable: true),
                    DefendSchedulePostChannel = table.Column<ulong>(nullable: true),
                    DefendSchedulePostTime = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alliances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ct_diplomatic_relation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 0),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ct_diplomatic_relation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ct_resources",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 0),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Label = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ct_resources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "alliance_diplomacy",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    OwnerId = table.Column<long>(nullable: false),
                    RelatedId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alliance_diplomacy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alliance_diplomacy_alliances_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_alliance_diplomacy_alliances_RelatedId",
                        column: x => x.RelatedId,
                        principalTable: "alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "zones",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Level = table.Column<int>(nullable: false),
                    Threats = table.Column<string>(maxLength: 2000, nullable: true),
                    DefendUtcDayOfWeek = table.Column<string>(maxLength: 15, nullable: false),
                    DefendUtcTime = table.Column<string>(maxLength: 10, nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    OwnerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_zones_alliances_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "starsystems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    ZoneId = table.Column<long>(nullable: false),
                    ResourceId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_starsystems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_starsystems_ct_resources_ResourceId",
                        column: x => x.ResourceId,
                        principalTable: "ct_resources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_starsystems_zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ct_diplomatic_relation",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 0, "Unspecified" },
                    { -1, "Enemy" },
                    { 1, "Neutral" },
                    { 2, "Friendly" },
                    { 3, "Allied" }
                });

            migrationBuilder.InsertData(
                table: "ct_resources",
                columns: new[] { "Id", "Label", "Name" },
                values: new object[,]
                {
                    { 34, "Crystal Tier 4", "CrystalTier4" },
                    { 33, "Crystal Tier 3", "CrystalTier3" },
                    { 24, "Gas Tier 4", "GasTier4" },
                    { 23, "Gas Tier 3", "GasTier3" },
                    { 13, "Isogen Tier 3", "IsogenTier3" },
                    { 3, "Dilithium", "Dilithium" },
                    { 11, "Isogen Tier 1", "IsogenTier1" },
                    { 43, "Ore Tier 3", "OreTier3" },
                    { 2, "Tritanium", "Tritanium" },
                    { 1, "Parasteel", "Parasteel" },
                    { 0, "Unspecified", "Unspecified" },
                    { 12, "Isogen Tier 2", "IsogenTier2" },
                    { 44, "Ore Tier 4", "OreTier4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_alliance_diplomacy_OwnerId",
                table: "alliance_diplomacy",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_alliance_diplomacy_RelatedId",
                table: "alliance_diplomacy",
                column: "RelatedId");

            migrationBuilder.CreateIndex(
                name: "IX_starsystems_ResourceId",
                table: "starsystems",
                column: "ResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_starsystems_ZoneId",
                table: "starsystems",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_zones_OwnerId",
                table: "zones",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alliance_diplomacy");

            migrationBuilder.DropTable(
                name: "ct_diplomatic_relation");

            migrationBuilder.DropTable(
                name: "starsystems");

            migrationBuilder.DropTable(
                name: "ct_resources");

            migrationBuilder.DropTable(
                name: "zones");

            migrationBuilder.DropTable(
                name: "alliances");
        }
    }
}
