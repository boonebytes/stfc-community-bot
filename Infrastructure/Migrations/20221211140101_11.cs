using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CT_JOB_STATUS",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false, defaultValue: 0),
                    NAME = table.Column<string>(maxLength: 200, nullable: false),
                    LABEL = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_JOB_STATUS", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOM_MESSAGE_JOBS",
                columns: table => new
                {
                    ID = table.Column<long>(nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    MODIFIED_BY = table.Column<string>(maxLength: 500, nullable: true),
                    MODIFIED_DATE = table.Column<DateTime>(nullable: false, defaultValueSql: "SYSDATE"),
                    SCHEDULED_TIMESTAMP = table.Column<DateTime>(nullable: false),
                    FROM_USER = table.Column<decimal>(nullable: false),
                    FROM_USERNAME = table.Column<string>(maxLength: 200, nullable: false),
                    FROM_USER_NICKNAME = table.Column<string>(maxLength: 200, nullable: true),
                    ALLIANCE_ID = table.Column<long>(nullable: true),
                    CHANNEL_ID = table.Column<decimal>(nullable: false),
                    MESSAGE = table.Column<string>(maxLength: 500, nullable: false),
                    JOB_STATUS_ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOM_MESSAGE_JOBS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CUSTOM_MESSAGE_JOBS_ALLIANCES_ALLIANCE_ID",
                        column: x => x.ALLIANCE_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "CT_JOB_STATUS",
                columns: new[] { "ID", "LABEL", "NAME" },
                values: new object[,]
                {
                    { 0, "Unspecified", "Unspecified" },
                    { 1, "Scheduled", "Scheduled" },
                    { 2, "Completed", "Completed" },
                    { 3, "Cancelled", "Cancelled" },
                    { 4, "Failed", "Failed" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOM_MESSAGE_JOBS_JOB_STATUS_ID",
                table: "CUSTOM_MESSAGE_JOBS",
                column: "JOB_STATUS_ID");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOM_MESSAGE_JOBS_ALLIANCE_ID",
                table: "CUSTOM_MESSAGE_JOBS",
                column: "ALLIANCE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CT_JOB_STATUS");

            migrationBuilder.DropTable(
                name: "CUSTOM_MESSAGE_JOBS");
        }
    }
}
