using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Grants;

[Authorize]
public class ReportModel(AppDbContext db, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : PageModel
{
    [BindProperty]
    public GrantReport Report { get; set; } = new();

    [BindProperty]
    public IFormFile? ReportFile { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var grant = await LoadGrantForCurrentUser(id);
        if (grant == null)
            return RedirectToPage("Index");

        // Autofill from the grant; the user fills in the rest.
        Report.GrantId = grant.Id;
        Report.ProjectDirector = grant.ProjectDirector?.Email ?? "";
        Report.ProjectTitle = grant.Title;
        Report.AwardDate = grant.AwardDate?.ToString("MM/dd/yyyy") ?? "";
        Report.ProjectSummary = grant.Description;

        var budgetTotal = grant.BudgetItems.Sum(b =>
            b.DepartmentAmount + b.CollegeAmount + b.ArccAmount + b.OtherAmount);
        if (budgetTotal > 0)
            Report.Budget = budgetTotal.ToString("C");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var grant = await LoadGrantForCurrentUser(Report.GrantId);
        if (grant == null)
            return RedirectToPage("Index");

        if (ReportFile != null && ReportFile.Length > 0
            && !Path.GetExtension(ReportFile.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(ReportFile), "The report file must be a pdf.");
        }

        if (!ModelState.IsValid)
            return Page();

        if (ReportFile != null && ReportFile.Length > 0)
        {
            var userId = userManager.GetUserId(User) ?? "anonymous";
            var folder = Path.Combine(env.WebRootPath, "uploads", userId);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(ReportFile.FileName)}";
            using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
            await ReportFile.CopyToAsync(stream);

            Report.ReportFile = $"/uploads/{userId}/{fileName}";
        }

        Report.SubmittedAt = DateTime.Now;
        db.GrantReports.Add(Report);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    // Only the project director may report, only on approved grants,
    // and only once per grant.
    private async Task<Grant?> LoadGrantForCurrentUser(int id)
    {
        var userId = userManager.GetUserId(User);
        if (userId == null)
            return null;

        var grant = await db.Grants
            .Include(g => g.ProjectDirector)
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id && g.ProjectDirectorId == userId);

        if (grant == null || !grant.IsApproved)
            return null;

        var alreadyReported = await db.GrantReports.AnyAsync(r => r.GrantId == id);
        return alreadyReported ? null : grant;
    }
}
