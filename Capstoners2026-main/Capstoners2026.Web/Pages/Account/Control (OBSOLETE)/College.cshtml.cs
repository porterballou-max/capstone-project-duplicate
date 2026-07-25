using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Pages.Account.Control
{
    [Authorize]
    public class CollegeModel(AppDbContext db, UserManager<ApplicationUser> userManager) : PageModel
    {
        [DisplayName("College")]
        [BindProperty]
        [Required]
        public int? CollegeId { get; set; }

        public List<SelectListItem> CollegeOptions { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCollegeOptions();
            return Page();
        }

        private async Task LoadCollegeOptions()
        {
            CollegeOptions = await db.Colleges
                    .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
                    .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCollegeOptions();
                return Page();
            }

            // Identify user and college 
            ApplicationUser applicationUser = await userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("Authenticated user not found.");
            College college = await db.Colleges.FindAsync(CollegeId)
                ?? throw new InvalidOperationException("Selected college not found.");

            // If user is already a member of this college, skip to department. 
            if (applicationUser.CollegeId == college.Id)
            {
                return RedirectToPage("/Account/Control/Department"); 
            }

            // Overwrite values 
            // Prevent user from belonging to a department of a different college. 
            applicationUser.DepartmentId = null;
            applicationUser.Department = null;
            applicationUser.CollegeId = college.Id;
            applicationUser.College = college;

            // Save changes 
            var result = await userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(String.Join(", ", result.Errors));
            }

            return RedirectToPage("/Account/Control/Department");
        }

    }
}
