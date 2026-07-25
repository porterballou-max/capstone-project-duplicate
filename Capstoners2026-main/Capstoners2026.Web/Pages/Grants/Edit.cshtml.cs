using Capstoners2026.Web.Data;
using Capstoners2026.Web.Models;
using Capstoners2026.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Capstoners2026.Web.Pages.Grants;

public class EditModel(
    AppDbContext db,
    UserManager<ApplicationUser> userManager, GrantService grantService, IWebHostEnvironment env) : PageModel
{
    private readonly AppDbContext _db = db;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public GrantViewModel GrantViewModel { get; set; } = new();

    [BindProperty]
    public Grant Grant { get; set; } = new();

    [BindProperty]
    public IFormFile? IrbApprovalFile { get; set; }

    [BindProperty]
    public string BudgetItemsJson { get; set; } = "";

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var grant = await _db.Grants
            .Include(g => g.BudgetItems)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grant == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isPrivileged = User.IsInRole("Admin") ||
                           User.IsInRole("Committee") ||
                           User.IsInRole("CommitteeChair") ||
                           User.IsInRole("DepartmentChair") ||
                           User.IsInRole("Dean");

        if (!isPrivileged && grant.ProjectDirectorId != userId)
            return Forbid();

        Grant = grant;

        var departmentOptions = await db.Departments
            .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
            .ToListAsync();

        var users = await _db.Users.OrderBy(u => u.Email).ToListAsync();
        var userOptions = new SelectList(users, nameof(ApplicationUser.Id), nameof(ApplicationUser.Email));

        GrantViewModel = new GrantViewModel
        {
            Grant = grant,
            IsReadOnly = false,
            BudgetItemsJson = grant.BudgetItems.Any()
                ? JsonSerializer.Serialize(grant.BudgetItems)
                : ""
        };
        await grantService.LoadOptions(_db, GrantViewModel);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {

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
            GrantViewModel = new GrantViewModel
            {
                Grant = Grant,
                IsReadOnly = false
            };
            await grantService.LoadOptions(_db, GrantViewModel);

            return Page();
        }

        if (IrbApprovalFile != null && IrbApprovalFile.Length > 0)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            var folder = Path.Combine(env.WebRootPath, "uploads", userId);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(IrbApprovalFile.FileName)}";
            using var stream = System.IO.File.Create(Path.Combine(folder, fileName));
            await IrbApprovalFile.CopyToAsync(stream);

            Grant.IrbApprovalFile = $"/uploads/{userId}/{fileName}";
        }

        // Handle budget items
        if (!string.IsNullOrWhiteSpace(BudgetItemsJson))
        {
            var items = JsonSerializer.Deserialize<List<BudgetItemInput>>(BudgetItemsJson);
            if (items != null)
            {
                Grant.BudgetItems = items.Select(item => new BudgetItem
                {
                    Title = item.Title,
                    ItemType = item.ItemType,
                    DepartmentAmount = item.DepartmentAmount,
                    CollegeAmount = item.CollegeAmount,
                    ArccAmount = item.ArccAmount,
                    OtherAmount = item.OtherAmount
                }).ToList();
            }
        }

        db.Attach(Grant).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
    public async Task<IActionResult> OnPostSubmitAsync()
    {

        if (!ModelState.IsValid)
        {
            GrantViewModel = new GrantViewModel
            {
                Grant = Grant,
                IsReadOnly = false
            };
            await grantService.LoadOptions(_db, GrantViewModel);
            return Page();
        }

        if (!Grant.Acknowledgement1 ||
            !Grant.Acknowledgement2 ||
            !Grant.Acknowledgement3 ||
            !Grant.Acknowledgement4)
        {
            ModelState.AddModelError(
                "",
                "All acknowledgements must be checked before submitting."
            );
        }

        if (string.IsNullOrWhiteSpace(Grant.ESignName))
        {
            ModelState.AddModelError(
                "Grant.ESignName",
                "Electronic signature is required."
            );
        }

        Grant.IsSubmitted = true;
        db.Attach(Grant).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}