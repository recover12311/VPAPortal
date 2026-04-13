using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPAPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddCrewsAndFlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Crews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AmmoItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrewId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AmmoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AmmoItems_Crews_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DroneItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrewId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DroneItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DroneItems_Crews_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Crews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Flights",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CrewId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    Coordinates = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DroneItemId = table.Column<int>(type: "int", nullable: false),
                    AmmoItemId = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    Settlement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flights", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Flights_AmmoItems_AmmoItemId",
                        column: x => x.AmmoItemId,
                        principalTable: "AmmoItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flights_Crews_CrewId",
                        column: x => x.CrewId,
                        principalTable: "Crews",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flights_DroneItems_DroneItemId",
                        column: x => x.DroneItemId,
                        principalTable: "DroneItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AmmoItems_CrewId",
                table: "AmmoItems",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_DroneItems_CrewId",
                table: "DroneItems",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_AmmoItemId",
                table: "Flights",
                column: "AmmoItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_CrewId",
                table: "Flights",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_Flights_DroneItemId",
                table: "Flights",
                column: "DroneItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flights");

            migrationBuilder.DropTable(
                name: "AmmoItems");

            migrationBuilder.DropTable(
                name: "DroneItems");

            migrationBuilder.DropTable(
                name: "Crews");
        }
    }
}
