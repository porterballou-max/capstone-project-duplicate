using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Capstoners2026.Web.Pages.Review;

[Authorize(Roles = "Committee,CommitteeChair")]
public class DetailsModel(AppDbContext db, UserManager<ApplicationUser> userManager) : PageModel
{
    public Grant Grant { get; set; } = new();
    public List<string> SubmittedFilePaths { get; set; } = [];
    public Data.Rubric? ActiveRubric { get; set; }  // <-- Fully qualify with Data.Rubric
    public GrantReview? ExistingReview { get; set; }

    [BindProperty]
    public Dictionary<int, int> CriteriaScores { get; set; } = new();

    [BindProperty]
    public string? ReviewNotes { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var grant = await db.Grants
            .Include(g => g.ProjectDirector)
            .Include(g => g.Department)
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grant == null)
            return NotFound();

        Grant = grant;

        if (!string.IsNullOrWhiteSpace(Grant.SubmittedFiles))
        {
            SubmittedFilePaths = JsonSerializer.Deserialize<List<string>>(Grant.SubmittedFiles) ?? [];
        }

        ActiveRubric = await db.Rubrics
            .Include(r => r.Criteria.OrderBy(c => c.DisplayOrder))
            .ThenInclude(c => c.ScoreOptions.OrderBy(so => so.Value))
            .FirstOrDefaultAsync();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            ExistingReview = await db.GrantReviews
                .Include(gr => gr.Scores)
                .FirstOrDefaultAsync(gr => gr.GrantId == id && gr.ReviewerId == userId);

            if (ExistingReview != null)
            {
                ReviewNotes = ExistingReview.Notes;
                foreach (var score in ExistingReview.Scores)
                {
                    CriteriaScores[score.RubricCriteriaId] = score.PointsAwarded;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var grant = await db.Grants
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grant == null)
            return NotFound();

        var rubric = await db.Rubrics
            .Include(r => r.Criteria)
            .ThenInclude(c => c.ScoreOptions)
            .FirstOrDefaultAsync();

        if (rubric == null)
        {
            ModelState.AddModelError("", "No active rubric found.");
            return await OnGetAsync(id);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Forbid();

        var totalPointsAwarded = 0;
        var totalMaxPoints = 0;

        var reviewScores = new List<GrantReviewScore>();

        foreach (var criteria in rubric.Criteria)
        {
            if (CriteriaScores.TryGetValue(criteria.Id, out var pointsAwarded))
            {
                var maxPoints = criteria.ScoreOptions.Max(so => so.Value);
                totalPointsAwarded += pointsAwarded;
                totalMaxPoints += maxPoints;

                reviewScores.Add(new GrantReviewScore
                {
                    RubricCriteriaId = criteria.Id,
                    PointsAwarded = pointsAwarded,
                    MaxPoints = maxPoints
                });
            }
        }

        var finalPercentage = totalMaxPoints > 0
            ? (decimal)totalPointsAwarded / totalMaxPoints * 100
            : 0;

        var existingReview = await db.GrantReviews
            .Include(gr => gr.Scores)
            .FirstOrDefaultAsync(gr => gr.GrantId == id && gr.ReviewerId == userId);

        if (existingReview != null)
        {
            db.GrantReviewScores.RemoveRange(existingReview.Scores);
            existingReview.FinalPercentage = finalPercentage;
            existingReview.Notes = ReviewNotes;
            existingReview.ReviewedDate = DateTime.UtcNow;
            existingReview.Scores = reviewScores;
        }
        else
        {
            var newReview = new GrantReview
            {
                GrantId = id,
                ReviewerId = userId,
                FinalPercentage = finalPercentage,
                Notes = ReviewNotes,
                Scores = reviewScores
            };
            db.GrantReviews.Add(newReview);
        }

        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    public decimal GetTotalAmount(Func<BudgetItem, decimal> selector)
    {
        return Grant.BudgetItems?.Sum(selector) ?? 0;
    }
}