using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Colleges;

public class IndexModel(AppDbContext db) : PageModel
{
    public List<College> Colleges { get; set; } = [];

    public async Task OnGetAsync()
    {
        Colleges = await db.Colleges.Include(c => c.Dean).ToListAsync();
    }
}