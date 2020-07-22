using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BSTServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SteamUsers",
                columns: table => new
                {
                    SteamUserId = table.Column<string>(nullable: false),
                    Nickname = table.Column<string>(nullable: true),
                    IsOnline = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SteamUsers", x => x.SteamUserId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    Role = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    SessionId = table.Column<Guid>(nullable: false),
                    SteamUserId = table.Column<string>(nullable: true),
                    ConnectTime = table.Column<DateTimeOffset>(nullable: true),
                    DisconnectTime = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionId);
                    table.ForeignKey(
                        name: "FK_Session_SteamUsers_SteamUserId",
                        column: x => x.SteamUserId,
                        principalTable: "SteamUsers",
                        principalColumn: "SteamUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionDamage",
                columns: table => new
                {
                    SessionDamageId = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    SteamUserId = table.Column<string>(nullable: true),
                    DamageTime = table.Column<DateTimeOffset>(nullable: false),
                    IsHurt = table.Column<bool>(nullable: false),
                    Damage = table.Column<int>(nullable: false),
                    Weapon = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionDamage", x => x.SessionDamageId);
                    table.ForeignKey(
                        name: "FK_SessionDamage_Session_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Session",
                        principalColumn: "SessionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionDamage_SteamUsers_SteamUserId",
                        column: x => x.SteamUserId,
                        principalTable: "SteamUsers",
                        principalColumn: "SteamUserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Session_SteamUserId",
                table: "Session",
                column: "SteamUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDamage_SessionId",
                table: "SessionDamage",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionDamage_SteamUserId",
                table: "SessionDamage",
                column: "SteamUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessionDamage");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "Session");

            migrationBuilder.DropTable(
                name: "SteamUsers");
        }
    }
}
