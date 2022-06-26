using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 64);

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 0,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Undefined", "Undefined" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Disabled", "Disabled" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Redundant", "Redundant" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Desired", "Desired" });

            migrationBuilder.InsertData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 4, "Preferred", "Preferred" },
                    { 5, "Basic", "Basic" }
                });

            migrationBuilder.UpdateData(
                table: "CT_DIPLOMATIC_RELATION",
                keyColumn: "ID",
                keyValue: -1,
                column: "NAME",
                value: "Pew");

            migrationBuilder.InsertData(
                table: "CT_DIPLOMATIC_RELATION",
                columns: new[] { "ID", "NAME" },
                values: new object[] { -99, "Enemy" });

            migrationBuilder.UpdateData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 64,
                column: "LABEL",
                value: "Progenitor Reactors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CT_DIPLOMATIC_RELATION",
                keyColumn: "ID",
                keyValue: -99);

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 0,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Unspecified", "Unspecified" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 1,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Parasteel", "Parasteel" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 2,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Tritanium", "Tritanium" });

            migrationBuilder.UpdateData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                keyColumn: "ID",
                keyValue: 3,
                columns: new[] { "LABEL", "NAME" },
                values: new object[] { "Dilithium", "Dilithium" });

            migrationBuilder.InsertData(
                table: "CT_ALLIANCE_SERVICE_LEVEL",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 64, "ProgenitorReactors", "ProgenitorReactors" },
                    { 63, "Progenitor Cores", "ProgenitorCores" },
                    { 62, "Progenitor Diodes", "ProgenitorDiodes" },
                    { 61, "Progenitor Emitters", "ProgenitorEmitters" },
                    { 53, "Refined Isogen Tier 3", "RefinedIsogenTier3" },
                    { 52, "Refined Isogen Tier 2", "RefinedIsogenTier2" },
                    { 44, "Ore Tier 4", "OreTier4" },
                    { 43, "Ore Tier 3", "OreTier3" },
                    { 34, "Crystal Tier 4", "CrystalTier4" },
                    { 33, "Crystal Tier 3", "CrystalTier3" },
                    { 24, "Gas Tier 4", "GasTier4" },
                    { 23, "Gas Tier 3", "GasTier3" },
                    { 13, "Isogen Tier 3", "IsogenTier3" },
                    { 12, "Isogen Tier 2", "IsogenTier2" },
                    { 51, "Refined Isogen Tier 1", "RefinedIsogenTier1" },
                    { 11, "Isogen Tier 1", "IsogenTier1" }
                });

            migrationBuilder.UpdateData(
                table: "CT_DIPLOMATIC_RELATION",
                keyColumn: "ID",
                keyValue: -1,
                column: "NAME",
                value: "Enemy");

            migrationBuilder.UpdateData(
                table: "CT_RESOURCES",
                keyColumn: "ID",
                keyValue: 64,
                column: "LABEL",
                value: "ProgenitorReactors");
        }
    }
}
