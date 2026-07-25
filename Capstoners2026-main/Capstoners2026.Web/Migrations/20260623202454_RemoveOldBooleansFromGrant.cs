using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldBooleansFromGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApprovedByCollegeDean",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "IsApprovedByDepartmentChair",
                table: "Grants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByCollegeDean",
                table: "Grants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsApprovedByDepartmentChair",
                table: "Grants",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
