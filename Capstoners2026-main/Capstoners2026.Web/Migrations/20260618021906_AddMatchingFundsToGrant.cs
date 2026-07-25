using Microsoft.EntityFrameworkCore.Migrations;
#nullable disable
namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddMatchingFundsToGrant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasMatchingFunds",
                table: "Grants",
                type: "bit",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<decimal>(
                name: "MatchingFundsAmount",
                table: "Grants",
                type: "decimal(18,2)",
                nullable: true);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasMatchingFunds",
                table: "Grants");
            migrationBuilder.DropColumn(
                name: "MatchingFundsAmount",
                table: "Grants");
        }
    }
}