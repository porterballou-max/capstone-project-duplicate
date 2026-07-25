using Capstoners2026.Web.Data;
using Capstoners2026.Web.Models;
using Capstoners2026.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Capstoners2026.Web.Pages;

public class IndexModel(AppDbContext context, UserManager<ApplicationUser> userManager, GrantService grantService) : PageModel
{

    // The current user's own grants, grouped by status.
    public GrantStatusSectionsViewModel MyGrants { get; set; } = new();

    // Stores grants of all departments of which the user is a chair. 
    [BindProperty]
    public Dictionary<Department, Grant[]> DepartmentGrants { get; set; } = [];
    [BindProperty]
    public Dictionary<College, Grant[]> CollegeGrants { get; set; } = [];

    // Stores total ARC money requested per grant 
    public Dictionary<Grant, decimal> ARCCBudget { get; set; } = [];
    // Stores total College money requested per grant 
    public Dictionary<Grant, decimal> CollegeBudget { get; set; } = [];

    public GrantsMissingReportsModel GrantsMissingReportsModel { get; set; }

    private async Task GetGrantsForDepartmentChair(ApplicationUser applicationUser)
    {
        // Fetch all departments of which the current user is a chair.
        var departments = await context.Departments.Where(d => d.ChairId == applicationUser.Id).ToArrayAsync();
        foreach (var d in departments)
        {
            var key = d;
            var value = await context.Grants.Where(
                    g => g.DepartmentId == d.Id 
                    && g.IsSubmitted == true 
                    && g.DepartmentChairApprovalStatus == Grant.ApprovalStatus.Pending)
                .Include(g => g.ProjectDirector)
                .Include(g => g.BudgetItems)
                .ToArrayAsync();
            DepartmentGrants.Add(key, value);

            // Calculate ARCC money requested by each grant 
            foreach (Grant grant in value)
            {
                decimal arccAmountSum = 0;
                foreach (var bi in grant.BudgetItems)
                {
                    arccAmountSum += bi.ArccAmount;
                }
                ARCCBudget.Add(grant, arccAmountSum);
            }
        }

        Console.WriteLine("##### " + DepartmentGrants.Values.Sum(v => v.Length) + " ####");

    }

    private async Task GetGrantsForCollegeDean(ApplicationUser applicationUser)
    {
        var colleges = await context.Colleges.Where(c => c.DeanId == applicationUser.Id).ToArrayAsync();

        foreach (var college in colleges)
        {
            var grants = new List<Grant>();
            // Find departments for this college 
            var departments = await context.Departments.Where(d => d.CollegeId == college.Id).ToArrayAsync();
            foreach (var department in departments)
            {
                Console.WriteLine("#### SCANNING " + department.Name + " ####");
                // Find grants from the given department which are awaiting dean approval 
                var g = await context.Grants
                    .Where(g => g.DepartmentId == department.Id
                            && g.RequiresDeanApproval
                            && g.CollegeDeanApprovalStatus == Grant.ApprovalStatus.Pending)
                    .ToArrayAsync();
                foreach (var gg in g)
                {
                    grants.Add(gg);
                    CollegeBudget.Add(gg, gg.BudgetItems.Sum(bi => bi.CollegeAmount));
                }
            }
            // Add to dictionary 
            CollegeGrants.Add(college, grants.ToArray());
        }


        // Log grants 
        var stringBuilder = new StringBuilder();
        foreach (var pair in CollegeGrants)
        {
            stringBuilder.AppendLine(String.Concat("#### ", pair.Key.Name, " | ", pair.Value.Count(), " ####"));
        }
        Console.WriteLine(stringBuilder.ToString());


    }

    public async Task<IActionResult> OnGetAsync()
    {

        ApplicationUser? applicationUser = await userManager.GetUserAsync(User);
        if (applicationUser != null)
        {
            GrantsMissingReportsModel = new GrantsMissingReportsModel();
            await GrantsMissingReportsModel.Load(context, applicationUser);
            MyGrants = await grantService.LoadStatusSections(context, applicationUser.Id);
            await GetGrantsForDepartmentChair(applicationUser);
            await GetGrantsForCollegeDean(applicationUser);
        }



        return Page();
    }


}
