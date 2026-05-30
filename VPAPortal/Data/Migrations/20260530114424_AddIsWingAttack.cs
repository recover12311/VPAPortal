using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPAPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddIsWingAttack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWingAttack",
                table: "DroneItems",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWingAttack",
                table: "DroneItems");
        }
    }
}
