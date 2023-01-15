using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordBot.Infrastructure.Migrations
{
    public partial class _12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "REACT_MESSAGES",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    POSTED = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    FROM_USER_ID = table.Column<decimal>(type: "NUMBER(20)", nullable: false),
                    FROM_USERNAME = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    GUILD_ID = table.Column<decimal>(type: "NUMBER(20)", nullable: false),
                    CHANNEL_ID = table.Column<decimal>(type: "NUMBER(20)", nullable: false),
                    DISCORD_MESSAGE_ID = table.Column<decimal>(type: "NUMBER(20)", nullable: true),
                    MESSAGE = table.Column<string>(type: "NCLOB", maxLength: 4000, nullable: false),
                    RESPONSETEXT = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ALLIANCE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    MODIFIED_BY = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    MODIFIED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REACT_MESSAGES", x => x.ID);
                    table.ForeignKey(
                        name: "FK_REACT_MESSAGES_ALLIANCES_ALLIANCE_ID",
                        column: x => x.ALLIANCE_ID,
                        principalTable: "ALLIANCES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "REACT_MESSAGE_REACTIONS",
                columns: table => new
                {
                    ID = table.Column<long>(type: "NUMBER(19)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    USER_ID = table.Column<decimal>(type: "NUMBER(20)", nullable: false),
                    USERNAME = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    NICKNAME = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    REACTION_RECEIVED = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    MESSAGE_ID = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    MODIFIED_BY = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    MODIFIED_DATE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false, defaultValueSql: "SYSDATE")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_REACT_MESSAGE_REACTIONS", x => x.ID);
                    table.ForeignKey(
                        name: "FK_REACT_MESSAGE_REACTIONS_REACT_MESSAGES_MESSAGE_ID",
                        column: x => x.MESSAGE_ID,
                        principalTable: "REACT_MESSAGES",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_REACT_MESSAGE_REACTIONS_MESSAGE_ID",
                table: "REACT_MESSAGE_REACTIONS",
                column: "MESSAGE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_REACT_MESSAGES_ALLIANCE_ID",
                table: "REACT_MESSAGES",
                column: "ALLIANCE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "REACT_MESSAGE_REACTIONS");

            migrationBuilder.DropTable(
                name: "REACT_MESSAGES");
        }
    }
}
