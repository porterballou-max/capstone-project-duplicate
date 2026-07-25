using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class ApprovalStatusEnumForGrants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CollegeDeanApprovalStatus",
                table: "Grants",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentChairApprovalStatus",
                table: "Grants",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollegeDeanApprovalStatus",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "DepartmentChairApprovalStatus",
                table: "Grants");
        }
    }
}
