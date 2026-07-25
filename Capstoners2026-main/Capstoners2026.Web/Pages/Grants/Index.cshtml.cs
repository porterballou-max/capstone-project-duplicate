using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Capstoners2026.Web.Pages.Grants;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Grant> Grants { get; set; } = [];

    public async Task OnGetAsync()
    {
        var isPrivileged = User.IsInRole("Admin") ||
                           User.IsInRole("Committee") ||
                           User.IsInRole("CommitteeChair") ||
                           User.IsInRole("DepartmentChair") ||
                           User.IsInRole("Dean");

        if (isPrivileged)
        {
            Grants = await db.Grants
                .Include(g => g.Department)
                .ToListAsync();
        }
        else
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Grants = await db.Grants
                .Include(g => g.Department)
                .Where(g => g.ProjectDirectorId == userId)
                .ToListAsync();
        }
    }
}