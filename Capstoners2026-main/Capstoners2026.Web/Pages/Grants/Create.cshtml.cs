using Capstoners2026.Web.Data;
using Capstoners2026.Web.Models;
using Capstoners2026.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;

namespace Capstoners2026.Web.Pages.Grants;

[Authorize]
public class CreateModel(AppDbContext db, UserManager<ApplicationUser> userManager, GrantService grantService, IWebHostEnvironment env) : PageModel
{
    [BindProperty] public GrantViewModel GrantViewModel { get; set; } = new(); 

    // Set after a draft is saved so the form can confirm it (Save button - Jake).
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        if (GrantViewModel == null)
        {
            GrantViewModel = new(); 
        }
        await grantService.LoadOptions(db, GrantViewModel); 
        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException();
        GrantViewModel.Grant.ProjectDirectorId = currentUser.Id;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await grantService.LoadOptions(db, GrantViewModel);
        var currentUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException();

        // Automatically set account number 
        if (GrantViewModel.Grant.ProjectDirectorId != null)
        {
            var projectDirector = await db.Users.Where(u => u.Id == GrantViewModel.Grant.ProjectDirectorId).FirstOrDefaultAsync();
            if (projectDirector != null && projectDirector.AccountNumber != null)
            {
                GrantViewModel.Grant.AccountNumber = projectDirector.AccountNumber; 
            }
        }

        if (!ModelState.IsValid)
        {

            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                });
            foreach (var x in errors)
            {
                foreach (var t in x.Errors)
                {
                    Console.WriteLine(t.ToString()); 
                }
            }


            Console.WriteLine("MODEL STATE IS INVALID"); 
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
        var savedPaths = new List<string>();

        foreach (var file in GrantViewModel.UploadedFiles.Take(3).Where(f => f.Length > 0))
        {
            var folder = Path.Combine(env.WebRootPath, "uploads", userId);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var path = Path.Combine(folder, fileName);

            using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);

            savedPaths.Add($"/uploads/{userId}/{fileName}");
        }

        if (savedPaths.Count > 0)
            GrantViewModel.Grant.SubmittedFiles = JsonSerializer.Serialize(savedPaths);

        if (GrantViewModel.IrbApprovalFile != null && GrantViewModel.IrbApprovalFile.Length > 0)
        {
            var folder = Path.Combine(env.WebRootPath, "uploads", userId);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(GrantViewModel.IrbApprovalFile.FileName)}";
            var path = Path.Combine(folder, fileName);

            using var stream = System.IO.File.Create(path);
            await GrantViewModel.IrbApprovalFile.CopyToAsync(stream);

            GrantViewModel.Grant.IrbApprovalFile = $"/uploads/{userId}/{fileName}";
        }

        List<BudgetItemInput>? items = null;

        if (!string.IsNullOrWhiteSpace(GrantViewModel.BudgetItemsJson))
        {
            items = JsonSerializer.Deserialize<List<BudgetItemInput>>(GrantViewModel.BudgetItemsJson);
        }

        if (items != null && items.Count > 0)
        {
            GrantViewModel.Grant.BudgetItems = items.Select(item => new BudgetItem
            {
                Title = item.Title,
                ItemType = item.ItemType,
                DepartmentAmount = item.DepartmentAmount,
                CollegeAmount = item.CollegeAmount,
                ArccAmount = item.ArccAmount,
                OtherAmount = item.OtherAmount
            }).ToList();
        }

        if (!GrantViewModel.Grant.Acknowledgement1 ||
            !GrantViewModel.Grant.Acknowledgement2 ||
            !GrantViewModel.Grant.Acknowledgement3 ||
            !GrantViewModel.Grant.Acknowledgement4)
        {
            ModelState.AddModelError(
                "",
                "All acknowledgements must be checked before submitting."
            );
        }

        if (string.IsNullOrWhiteSpace(GrantViewModel.Grant.ESignName))
        {
            ModelState.AddModelError(
                "",
                "Electronic signature is required."
            );
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }


        GrantViewModel.Grant.IsSubmitted = true;
        db.Grants.Add(GrantViewModel.Grant);
        await db.SaveChangesAsync();

        return RedirectToPage("Index");
    }

    // Save draft: persist the grant without enforcing validation, so partially
    // filled work isn't lost. Inserts a new grant, or updates the existing one
    // when re-saving (Id flows back via the hidden field in the form).
    public async Task<IActionResult> OnPostSaveAsync()
    {
        await grantService.LoadOptions(db, GrantViewModel); 

        if (GrantViewModel.Grant.Id == 0)
            db.Grants.Add(GrantViewModel.Grant);
        else
            db.Grants.Update(GrantViewModel.Grant);

        await db.SaveChangesAsync();
        StatusMessage = "Draft saved.";
        return Page();
    }
}