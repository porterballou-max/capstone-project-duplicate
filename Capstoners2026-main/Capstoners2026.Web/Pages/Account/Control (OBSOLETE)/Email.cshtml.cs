using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Pages.Account.Control
{
    [Authorize]
    public class EmailModel(UserManager<ApplicationUser> userManager) : PageModel
    {

        [DisplayName("New Email")]
        [BindProperty]
        [Required]
        public string NewEmail { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {

            // Overwrite email 
            ApplicationUser applicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Failed to identify authenticated user.");
            applicationUser.Email = NewEmail;
            // Save changes 
            var result = await userManager.UpdateAsync(applicationUser);

            if (result.Succeeded)
            {
                return RedirectToPage("/Account/Profile"); 
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

        }

    }
}
