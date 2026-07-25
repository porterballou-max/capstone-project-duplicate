using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Capstoners2026.Web.Pages.Account;

public class RegisterModel(AppDbContext db, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) : PageModel
{

    [BindProperty]
    [Required]
    public required string Email { get; set; }

    [BindProperty]
    [Required]
    public required string Password { get; set; }

    [BindProperty]
    [Required]
    public required string ConfirmPassword { get; set; }

    [BindProperty]
    public int? CollegeId { get; set; }

    [BindProperty]
    public int? DepartmentId { get; set; }

    public List<SelectListItem> CollegeOptions { get; set; } = [];
    public List<SelectListItem> DepartmentOptions { get; set; } = [];

    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

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

        ApplicationUser user = new ApplicationUser
        {
            Email = this.Email,
            UserName = this.Email,
            CollegeId = this.CollegeId,
            DepartmentId = this.DepartmentId
        };
        
        var result = await _userManager.CreateAsync(user, Password);

        if (result.Succeeded)
        {
            // Try signing in as given user 
            await _signInManager.SignInAsync(user, true); 
            return RedirectToPage("/Grants/Index");
        }

        Console.WriteLine(result.Errors.ToString()); 

        ModelState.AddModelError("", "Registration failure!");
        foreach (var x in result.Errors)
        {
            ModelState.AddModelError("", x.Description);
        }

        await LoadCollegeOptions(); 
        return Page();

    }

    // ==================== NAMED HANDLER ====================
    public async Task<JsonResult> OnGetGetDepartments(int? collegeId)
    {
        if (collegeId == null)
        {
            return new JsonResult(new List<object>());
        }

        var departments = await db.Departments
            .Where(d => d.CollegeId == collegeId)   // assuming you have this relationship
            .Select(d => new { id = d.Id, name = d.Name })
            .ToListAsync();

        return new JsonResult(departments);
    }
}
