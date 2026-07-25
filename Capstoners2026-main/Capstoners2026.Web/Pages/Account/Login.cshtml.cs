using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Pages.Account;

public class LoginModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : PageModel
{

    [BindProperty]
    [Required]
    public required string Email { get; set; }

    [BindProperty]
    [Required]
    public required string Password { get; set; }

    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await _signInManager.PasswordSignInAsync(
            Email,
            Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByEmailAsync(Email);

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                return RedirectToPage("/Index");
            }

            return RedirectToPage("/Grants/Index");
        }

        ModelState.AddModelError("", "Invalid login attempt.");
        return Page();
    }

}
