using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstoners2026.Web.Pages.Review;

[Authorize(Roles = "Committee,CommitteeChair")]
public class IndexModel(AppDbContext db) : PageModel
{
    public List<Grant> Grants { get; set; } = [];
    public Dictionary<int, List<GrantReview>> GrantReviews { get; set; } = new();
    public Dictionary<int, GrantReview?> MyReviews { get; set; } = new();

    public async Task OnGetAsync()
    {
        Grants = await db.Grants
            .Where(g => g.IsSubmitted)
            .Include(g => g.ProjectDirector)
            .Include(g => g.BudgetItems)
            .ToListAsync();

        var grantIds = Grants.Select(g => g.Id).ToList();

        var allReviews = await db.GrantReviews
            .Where(gr => grantIds.Contains(gr.GrantId))
            .Include(gr => gr.Reviewer)
            .ToListAsync();

        foreach (var grant in Grants)
        {
            GrantReviews[grant.Id] = allReviews.Where(r => r.GrantId == grant.Id).ToList();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            foreach (var grant in Grants)
            {
                MyReviews[grant.Id] = allReviews.FirstOrDefault(r => r.GrantId == grant.Id && r.ReviewerId == userId);
            }
        }
    }

    public decimal GetArccTotal(Grant grant)
    {
        return grant.BudgetItems?.Sum(b => b.ArccAmount) ?? 0;
    }

    public decimal GetAverageScore(int grantId)
    {
        if (GrantReviews.TryGetValue(grantId, out var reviews) && reviews.Any())
        {
            return reviews.Average(r => r.FinalPercentage);
        }
        return 0;
    }
}