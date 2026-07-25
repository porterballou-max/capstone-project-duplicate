using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Capstoners2026.Web.Pages.Account;

public class LogoutModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

    public async Task<IActionResult> OnGetAsync()
    {
        await _signInManager.SignOutAsync();

        return RedirectToPage("/Index");
    }
}