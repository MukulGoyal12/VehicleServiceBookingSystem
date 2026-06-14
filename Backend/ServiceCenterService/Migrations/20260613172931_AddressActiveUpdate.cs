using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCenterService.Migrations
{
    /// <inheritdoc />
    public partial class AddressActiveUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ServiceCenters",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "ServiceCenters",
                newName: "City");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceDescription",
                table: "ServiceCenters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "ServiceCenters",
                type: "varchar(20)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "ServiceCenters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CenterName",
                table: "ServiceCenters",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ServiceCenters",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FlatNumber",
                table: "ServiceCenters",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NearestLandmark",
                table: "ServiceCenters",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Pincode",
                table: "ServiceCenters",
                type: "varchar(6)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ServiceCenters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "ServiceCenters",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "CenterName",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "FlatNumber",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "NearestLandmark",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "Pincode",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ServiceCenters");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "ServiceCenters");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "ServiceCenters",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "ServiceCenters",
                newName: "Location");

            migrationBuilder.AlterColumn<string>(
                name: "ServiceDescription",
                table: "ServiceCenters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "ServiceCenters",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");
        }
    }
}
