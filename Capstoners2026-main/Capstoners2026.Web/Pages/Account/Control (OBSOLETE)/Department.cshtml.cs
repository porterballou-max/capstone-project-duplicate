using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Pages.Account.Control
{
    [Authorize]
    public class DepartmentModel(AppDbContext db, UserManager<ApplicationUser> userManager) : PageModel
    {

        [BindProperty]
        [Required]
        public int? DepartmentId { get; set; }

        public List<SelectListItem> DepartmentOptions { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {

            Console.WriteLine("Hello department menu");

            var applicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Failed to identify authenticated user.");
            // Require user to belong to a college 
            if (applicationUser.CollegeId == null)
            {
                return RedirectToPage("/Account/Control/College");
            }

            await LoadDepartmentOptions(applicationUser.CollegeId);
            return Page();
        }

        private async Task LoadDepartmentOptions(int? collegeId)
        {
            DepartmentOptions = await db.Departments
                .Where(d => d.CollegeId == collegeId)
                .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var applicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Failed to identify authenticated user.");

            if (!ModelState.IsValid)
            {
                await LoadDepartmentOptions(applicationUser.CollegeId);
                return Page();
            }

            Department department = await db.Departments.FindAsync(DepartmentId) ?? throw new InvalidOperationException("Invalid department selected");

            // Overwrite
            applicationUser.DepartmentId = department.Id;
            applicationUser.Department = department;

            // Save changes 
            var result = await userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(String.Join(", ", result.Errors));
            }

            return RedirectToPage("/Account/Profile"); 

        }

    }
}
