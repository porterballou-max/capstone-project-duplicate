using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Capstoners2026.Web.Migrations
{
    /// <inheritdoc />
    public partial class FixRubricNamespaceConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GrantReviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrantId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FinalPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrantReviews_AspNetUsers_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrantReviews_Grants_GrantId",
                        column: x => x.GrantId,
                        principalTable: "Grants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrantReviewScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrantReviewId = table.Column<int>(type: "int", nullable: false),
                    RubricCriteriaId = table.Column<int>(type: "int", nullable: false),
                    PointsAwarded = table.Column<int>(type: "int", nullable: false),
                    MaxPoints = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrantReviewScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrantReviewScores_GrantReviews_GrantReviewId",
                        column: x => x.GrantReviewId,
                        principalTable: "GrantReviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GrantReviewScores_RubricCriteria_RubricCriteriaId",
                        column: x => x.RubricCriteriaId,
                        principalTable: "RubricCriteria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviews_GrantId",
                table: "GrantReviews",
                column: "GrantId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviews_ReviewerId",
                table: "GrantReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviewScores_GrantReviewId",
                table: "GrantReviewScores",
                column: "GrantReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_GrantReviewScores_RubricCriteriaId",
                table: "GrantReviewScores",
                column: "RubricCriteriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrantReviewScores");

            migrationBuilder.DropTable(
                name: "GrantReviews");
        }
    }
}
