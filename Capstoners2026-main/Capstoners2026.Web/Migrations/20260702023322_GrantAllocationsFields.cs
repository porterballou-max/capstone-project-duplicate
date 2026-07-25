using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class GrantAllocationsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AllocatedAmount",
                table: "Grants",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AllocationFinalized",
                table: "Grants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportingDueDate",
                table: "Grants",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ReviewerScore",
                table: "Grants",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "GrantId1",
                table: "GrantReviews",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AllocationRules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllocationRoundId = table.Column<int>(type: "int", nullable: false),
                    MinimumScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaximumScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FundingPercent = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllocationRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AllocationRules_AllocationRounds_AllocationRoundId",
                        column: x => x.AllocationRoundId,
                        principalTable: "AllocationRounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviews_GrantId1",
                table: "GrantReviews",
                column: "GrantId1");

            migrationBuilder.CreateIndex(
                name: "IX_AllocationRules_AllocationRoundId",
                table: "AllocationRules",
                column: "AllocationRoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_GrantReviews_Grants_GrantId1",
                table: "GrantReviews",
                column: "GrantId1",
                principalTable: "Grants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GrantReviews_Grants_GrantId1",
                table: "GrantReviews");

            migrationBuilder.DropTable(
                name: "AllocationRules");

            migrationBuilder.DropIndex(
                name: "IX_GrantReviews_GrantId1",
                table: "GrantReviews");

            migrationBuilder.DropColumn(
                name: "AllocatedAmount",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "AllocationFinalized",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "ReportingDueDate",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "ReviewerScore",
                table: "Grants");

            migrationBuilder.DropColumn(
                name: "GrantId1",
                table: "GrantReviews");
        }
    }
}
