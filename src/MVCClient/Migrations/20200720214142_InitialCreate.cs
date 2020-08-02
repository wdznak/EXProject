using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCClient.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Connections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(maxLength: 15, nullable: true),
                    Exchange = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connections", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Whale1Hs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ConnectionID = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Market = table.Column<float>(nullable: false),
                    Limit = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Whale1Hs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Whale1Hs_Connections_ConnectionID",
                        column: x => x.ConnectionID,
                        principalTable: "Connections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Whale1Hs_ConnectionID",
                table: "Whale1Hs",
                column: "ConnectionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Whale1Hs");

            migrationBuilder.DropTable(
                name: "Connections");
        }
    }
}
