using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceCenterService.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ServiceDescription",
                table: "ServiceCenters",
                type: "nvarchar(1100)",
                maxLength: 1100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ServiceDescription",
                table: "ServiceCenters",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1100)",
                oldMaxLength: 1100);
        }
    }
}
