using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddGrantReportsAndAwardDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantReviews_Grants_GrantId1",
                table: "GrantReviews");

            migrationBuilder.DropIndex(
                name: "IX_GrantReviews_GrantId1",
                table: "GrantReviews");

            migrationBuilder.DropColumn(
                name: "GrantId1",
                table: "GrantReviews");

            migrationBuilder.AddColumn<DateTime>(
                name: "AwardDate",
                table: "Grants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GrantReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrantId = table.Column<int>(type: "int", nullable: false),
                    ProjectDirector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AwardDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectSummary = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentProgress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NextSteps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Budget = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrantReports_Grants_GrantId",
                        column: x => x.GrantId,
                        principalTable: "Grants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrantReports_GrantId",
                table: "GrantReports",
                column: "GrantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrantReports");

            migrationBuilder.DropColumn(
                name: "AwardDate",
                table: "Grants");

            migrationBuilder.AddColumn<int>(
                name: "GrantId1",
                table: "GrantReviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviews_GrantId1",
                table: "GrantReviews",
                column: "GrantId1");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantReviews_Grants_GrantId1",
                table: "GrantReviews",
                column: "GrantId1",
                principalTable: "Grants",
                principalColumn: "Id");
        }
    }
}
