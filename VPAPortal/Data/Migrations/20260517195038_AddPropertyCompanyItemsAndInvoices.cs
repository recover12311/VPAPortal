using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPAPortal.Migrations
{
    /// <inheritdoc />
    public partial class AddPropertyCompanyItemsAndInvoices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "PropertyLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "PropertyLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CrewId",
                table: "PropertyLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PropertyLogs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "PropertyLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "PropertyLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "PropertyLogs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CompanyPropertyItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PropertyTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyPropertyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyPropertyItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyPropertyItems_PropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "PropertyTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    PhotoPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyInvoices_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLogs_CompanyId",
                table: "PropertyLogs",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyLogs_CrewId",
                table: "PropertyLogs",
                column: "CrewId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPropertyItems_CompanyId",
                table: "CompanyPropertyItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyPropertyItems_PropertyTypeId",
                table: "CompanyPropertyItems",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyInvoices_CompanyId",
                table: "PropertyInvoices",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyLogs_Companies_CompanyId",
                table: "PropertyLogs",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyLogs_Crews_CrewId",
                table: "PropertyLogs",
                column: "CrewId",
                principalTable: "Crews",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyLogs_Companies_CompanyId",
                table: "PropertyLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_PropertyLogs_Crews_CrewId",
                table: "PropertyLogs");

            migrationBuilder.DropTable(
                name: "CompanyPropertyItems");

            migrationBuilder.DropTable(
                name: "PropertyInvoices");

            migrationBuilder.DropIndex(
                name: "IX_PropertyLogs_CompanyId",
                table: "PropertyLogs");

            migrationBuilder.DropIndex(
                name: "IX_PropertyLogs_CrewId",
                table: "PropertyLogs");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "PropertyLogs");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "PropertyLogs");

            migrationBuilder.AlterColumn<string>(
                name: "ItemName",
                table: "PropertyLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Details",
                table: "PropertyLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<int>(
                name: "CrewId",
                table: "PropertyLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "PropertyLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "PropertyLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
