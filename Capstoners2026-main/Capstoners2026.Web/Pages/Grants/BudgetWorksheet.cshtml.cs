using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Grants;

[Authorize]
public class BudgetWorksheetModel(AppDbContext db) : PageModel
{
    public Grant Grant { get; set; } = new();

    [BindProperty]
    public BudgetItem NewBudgetItem { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var grant = await db.Grants
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grant == null)
            return NotFound();

        Grant = grant;

        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync(int id)
    {
        var grant = await db.Grants
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grant == null)
            return NotFound();

        NewBudgetItem.GrantId = id;
        ModelState.Remove("NewBudgetItem.Grant");

        if (!ModelState.IsValid)
        {
            Grant = grant;
            return Page();
        }

        db.BudgetItems.Add(NewBudgetItem);
        await db.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, int budgetItemId)
    {
        var item = await db.BudgetItems
            .FirstOrDefaultAsync(b => b.Id == budgetItemId && b.GrantId == id);

        if (item == null)
            return NotFound();

        db.BudgetItems.Remove(item);
        await db.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}