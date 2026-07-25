using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Capstoners2026.Web.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;

namespace Capstoners2026.Web.Pages.Grants
{
    /// <summary>
    /// Used to review/approve grants. 
    /// </summary>
    /// <param name="context"></param>
    [Authorize]
    public class ApproveModel(AppDbContext context, UserManager<ApplicationUser> userManager, IWebHostEnvironment env) : PageModel
    {

        public enum ApprovalContext
        {
            None,
            DepartmentChair,
            CollegeDean
        }
        [BindProperty]
        public ApprovalContext AssignedApprovalContext { get; set; }

        private ApplicationUser applicationUser = new();

        [BindProperty]
        public GrantViewModel GrantViewModel { get; set; } = new GrantViewModel { IsReadOnly = true };

        [BindProperty]
        public int GrantId { get; set; }

        /// <summary>
        /// Verifies that the signed in user is a chair for the department of which the grant belongs. 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ValidateChair(ApplicationUser u, Grant g)
        {
            Console.WriteLine("#### VALIDATING CHAIR ####");
            Console.WriteLine("#### " + u.Id + ", " + g.Id + " ####");
            var uChairs = await context.Departments.Where(d => d.Id == g.DepartmentId && d.ChairId == u.Id).ToArrayAsync();
            if (!uChairs.IsNullOrEmpty())
            {
                return true; 
            }
            throw new Exception("Invalid authority"); 
        }

        /// <summary>
        /// Verifies that the signed in user is a dean for the college of which the grant belongs. 
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ValidateDean(ApplicationUser u, Grant g)
        {
            var uDeans = await context.Colleges.Where(c => c.Id == g.Department.CollegeId && c.DeanId == u.Id).ToArrayAsync();
            if (!uDeans.IsNullOrEmpty())
            {
                return true;
            }
            throw new Exception("Invalid authority");
        }

        /// <summary>
        /// Records the award date once the last required approval comes in.
        /// </summary>
        private static void StampAwardDate(Grant g)
        {
            if (g.IsApproved && g.AwardDate == null)
            {
                g.AwardDate = DateTime.Now;
            }
        }

        // "id" parameter refers to grant id
        public async Task<IActionResult> OnGetAsync(int id, int role)
        {

            GrantId = id; 
            applicationUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User not found");

            // Load grant based on ID 
            GrantViewModel.Grant = await context.Grants.Where(c => c.Id == id)
                .Include(g => g.BudgetItems)
                .Include(g => g.Department)
                .Include(g => g.ProjectDirector)
                .FirstOrDefaultAsync() ?? throw new NullReferenceException("Tried reviewing a grant that does not exist.");

            // Authorize user based on value of "role" argument 
            if (role == 0)
            {
                // Validate that the user actually is a chair of the corresponding department. 
                Console.WriteLine("#### AS CHAIR ####");
                await ValidateChair(applicationUser, GrantViewModel.Grant);
                AssignedApprovalContext = ApprovalContext.DepartmentChair;
                Console.WriteLine("#### AUTHENTICATION SUCCESSFUL ####");
            }
            else if (role == 1)
            {
                // Validate that the user actually is a dean of the corresponding college. 
                Console.WriteLine("#### AS DEAN ####");
                await ValidateDean(applicationUser, GrantViewModel.Grant);
                AssignedApprovalContext = ApprovalContext.CollegeDean;
                Console.WriteLine("#### AUTHENTICATION SUCCESSFUL ####");
            }
            else
            {
                throw new Exception("Invalid review mode selected.");
            }

            return Page(); 
        }

        /// <summary>
        /// 0 = Approve
        /// 1 = Reject 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnPostAsync(int decision)
        {

            applicationUser = await userManager.GetUserAsync(User) ?? throw new NullReferenceException("User not found");

            // Load grant based on ID 
            GrantViewModel.Grant = await context.Grants.Where(c => c.Id == GrantId)
                .Include(g => g.BudgetItems)
                .Include(g => g.Department)
                .Include(g => g.ProjectDirector)
                .FirstOrDefaultAsync() ?? throw new NullReferenceException("Tried reviewing a grant that does not exist.");

            // Approve
            if (decision == 0)
            {

                switch (AssignedApprovalContext)
                {
                    case ApprovalContext.DepartmentChair:
                        // Double check authority
                        await ValidateChair(applicationUser, GrantViewModel.Grant);
                        // Apply changes
                        GrantViewModel.Grant.DepartmentChairApprovalStatus = Grant.ApprovalStatus.Approved;
                        StampAwardDate(GrantViewModel.Grant);
                        await context.SaveChangesAsync();
                        break;
                    case ApprovalContext.CollegeDean:
                        // Double check authority
                        await ValidateDean(applicationUser, GrantViewModel.Grant);
                        GrantViewModel.Grant.CollegeDeanApprovalStatus = Grant.ApprovalStatus.Approved;
                        StampAwardDate(GrantViewModel.Grant);
                        // Apply changes
                        await context.SaveChangesAsync();
                        break;
                    default:
                        throw new Exception("Invalid ApprovalContext."); 
                }

            }
            // Reject 
            else if (decision == 1)
            {

                switch (AssignedApprovalContext)
                {
                    case ApprovalContext.DepartmentChair:
                        // Double check authority
                        await ValidateChair(applicationUser, GrantViewModel.Grant);
                        // Apply changes 
                        GrantViewModel.Grant.DepartmentChairApprovalStatus = Grant.ApprovalStatus.Rejected;
                        await context.SaveChangesAsync();
                        break;
                    case ApprovalContext.CollegeDean:
                        // Double check authority
                        await ValidateDean(applicationUser, GrantViewModel.Grant);
                        GrantViewModel.Grant.CollegeDeanApprovalStatus = Grant.ApprovalStatus.Rejected;
                        // Apply changes 
                        await context.SaveChangesAsync();
                        break;
                    default:
                        throw new Exception("Invalid ApprovalContext.");
                }

            }
            //// Send to accounting
            //else if (decision == 2)
            //{
            //    Console.WriteLine("#### SEND TO ACCOUNTING ####");
            //    ModelState.Clear(); // temp fix 

            //    ExcelPackage.License.SetNonCommercialOrganization("Capstoners2026");
            //    using (var package = new ExcelPackage())
            //    {
            //        // Add a new worksheet
            //        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("MySheet");

            //        // Write data (note: EPPlus uses 1-based indexing)
            //        worksheet.Cells[1, 1].Value = "Principal Investigator Name";    // A1
            //        worksheet.Cells[1, 2].Value = "Grant Title";                    // B1
            //        worksheet.Cells[1, 3].Value = "Total Requested";                // C1

            //        // Add some sample rows
            //        // A2
            //        worksheet.Cells[2, 1].Value = GrantViewModel.Grant.ProjectDirector != null ? GrantViewModel.Grant.ProjectDirector.UserName : "Unknown";
            //        // B2
            //        worksheet.Cells[2, 2].Value = GrantViewModel.Grant.Title;
            //        // C2
            //        worksheet.Cells[2, 3].Value = GrantViewModel.Grant.BudgetItems.Sum(g => g.ArccAmount+g.CollegeAmount+g.DepartmentAmount+g.OtherAmount);

            //        // Formatting
            //        worksheet.Cells["A1:D1"].Style.Font.Bold = true;  // Bold headers
            //        worksheet.Cells["C2:C3"].Style.Numberformat.Format = "$#,##0.00"; // Currency format

            //        // Auto-fit columns
            //        worksheet.Cells.AutoFitColumns();

            //        using var stream = new MemoryStream();
            //        package.SaveAs(stream);
            //        stream.Position = 0;

            //        var fileName = GrantViewModel.Grant.Title + "(" + DateTime.Now.ToString() + ").xlsx";

            //        return File(stream.ToArray(),
            //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            //            fileName);

            //    }

            //    return Page(); 
            //}
            else
            {
                throw new Exception("Tried to make an invalid decision on a grant."); 
            }

            // Return to dashboard 
            return RedirectToPage("/Index"); 
        }

    }
}
