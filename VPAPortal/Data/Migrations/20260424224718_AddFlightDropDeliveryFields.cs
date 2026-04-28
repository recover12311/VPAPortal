using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPAPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightDropDeliveryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AmmoItemId",
                table: "FlightDrops",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "DeliveryTime",
                table: "FlightDrops",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivery",
                table: "FlightDrops",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Settlement",
                table: "FlightDrops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTime",
                table: "FlightDrops");

            migrationBuilder.DropColumn(
                name: "IsDelivery",
                table: "FlightDrops");

            migrationBuilder.DropColumn(
                name: "Settlement",
                table: "FlightDrops");

            migrationBuilder.AlterColumn<int>(
                name: "AmmoItemId",
                table: "FlightDrops",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
