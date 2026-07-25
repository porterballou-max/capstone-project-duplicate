using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Departments;

public class EditModel(AppDbContext db) : PageModel
{
    [BindProperty]
    public Department Department { get; set; } = new();

    public List<SelectListItem> CollegeOptions { get; set; } = [];
    public List<SelectListItem> UserOptions { get; set; } = [];

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var department = await db.Departments.FindAsync(id);
        if (department == null) return NotFound();
        Department = department;

        CollegeOptions = await db.Colleges
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

        UserOptions = await db.Users
            .Select(u => new SelectListItem(u.Email, u.Id.ToString()))
            .ToListAsync();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            CollegeOptions = await db.Colleges
                .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                .ToListAsync();
            UserOptions = await db.Users
                .Select(u => new SelectListItem(u.Email, u.Id.ToString()))
                .ToListAsync();
            return Page();
        }

        db.Attach(Department).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
