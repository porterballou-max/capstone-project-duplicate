using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountNumberToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "AspNetUsers",
                type: "nvarchar(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "AspNetUsers");
        }
    }
}
