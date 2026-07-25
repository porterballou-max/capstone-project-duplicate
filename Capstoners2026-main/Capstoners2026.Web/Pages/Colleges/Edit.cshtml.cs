using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Colleges;

public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public College College { get; set; } = new();

    public List<SelectListItem> UserOptions { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var college = await db.Colleges.FindAsync(id);
        if (college == null) return NotFound();
        College = college;

        UserOptions = await db.Users
            .Select(u => new SelectListItem(u.Email, u.Id.ToString()))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            UserOptions = await db.Users
                .Select(u => new SelectListItem(u.Email, u.Id.ToString()))
                .ToListAsync();
            return Page();
        }

        db.Attach(College).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
