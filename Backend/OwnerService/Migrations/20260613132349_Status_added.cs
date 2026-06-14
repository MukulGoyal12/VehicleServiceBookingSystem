using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OwnerService.Migrations
{
    /// <inheritdoc />
    public partial class Status_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Owners");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Owners",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Owners");

            migrationBuilder.AddColumn<string>(
                name: "IsActive",
                table: "Owners",
                type: "nvarchar(1)",
                nullable: false,
                defaultValue: "");
        }
    }
}
