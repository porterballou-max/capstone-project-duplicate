using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Departments;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<Department> Departments { get; set; } = [];

    public async Task OnGetAsync()
    {
        Departments = await db.Departments
            .Include(d => d.College)
            .Include(d => d.Chair)
            .ToListAsync();
    }
}