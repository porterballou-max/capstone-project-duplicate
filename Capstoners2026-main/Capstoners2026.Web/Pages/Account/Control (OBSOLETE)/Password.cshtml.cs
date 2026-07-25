using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Pages.Account.Control
{
    public class PasswordModel(UserManager<ApplicationUser> userManager) : PageModel
    {
        [DisplayName("Old Password")]
        [BindProperty]
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [DisplayName("New Password")]
        [BindProperty]
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [DisplayName("Confirm New Password")]
        [BindProperty]
        [Required]
        public string ConfirmNewPassword { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {

            ApplicationUser applicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Failed to identify authenticated user.");
            string oldHash = userManager.PasswordHasher.HashPassword(applicationUser, OldPassword); 

            // Confirmed password incorrectly
            if (NewPassword != ConfirmNewPassword)
            {
                ModelState.AddModelError("", "Failed to confirm new password."); 
            }

            var result = await userManager.ChangePasswordAsync(applicationUser, OldPassword, ConfirmNewPassword);

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
