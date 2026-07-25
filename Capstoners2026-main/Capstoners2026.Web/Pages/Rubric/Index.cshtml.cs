using Microsoft.AspNetCore.Mvc.RazorPages;
using Capstoners2026.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Capstoners2026.Web.Pages.Rubric
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Data.Rubric> Rubrics { get; set; } = new();

        public async Task OnGetAsync()
        {
            Rubrics = await _context.Rubrics
                .Include(r => r.Criteria)
                .ToListAsync();
        }
    }
}