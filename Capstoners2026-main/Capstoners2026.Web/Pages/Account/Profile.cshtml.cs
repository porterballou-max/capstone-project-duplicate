using Capstoners2026.Web.Data;
using Capstoners2026.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Capstoners2026.Web.Pages.Account
{

    [Authorize]
    public class ProfileModel(AppDbContext db, UserManager<ApplicationUser> userManager) : PageModel
    {
        [BindProperty]
        public required ResetEmailModel ResetEmailModel { get; set; }
        [BindProperty]
        public required ResetPasswordModel ResetPasswordModel { get; set; }
        [BindProperty]
        public required SelectCollegeModel SelectCollegeModel { get; set; }
        [BindProperty]
        public required SelectDepartmentModel SelectDepartmentModel { get; set; }
        [BindProperty]
        public required SelectAccountNumberModel SelectAccountNumberModel { get; set; }
        public required ApplicationUser ApplicationUser { get; set; }

        [BindProperty]
        public string CollegeName { get; set; } = "Loading...";
        [BindProperty]
        public string DepartmentName { get; set; } = "Loading...";
        [BindProperty]
        public string AccountNumber { get; set; } = "Loading..."; 

        public async Task LoadApplicationUserAsync()
        {
            ApplicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Could not identify the authenticated user.");
        }

        public async Task LoadDataAsync()
        {
            ApplicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Could not identify the authenticated user.");

            ResetPasswordModel = new ResetPasswordModel();
            ResetEmailModel = new ResetEmailModel();

            // Load college 
            await db.Entry(ApplicationUser)
            .Reference(u => u.College)
            .LoadAsync();

            // Load department 
            await db.Entry(ApplicationUser)
            .Reference(u => u.Department)
            .LoadAsync();

            SelectCollegeModel =
                new SelectCollegeModel
                {
                    CollegeId = ApplicationUser.CollegeId,
                    CollegeOptions = await db.Colleges.Select(d => new SelectListItem(d.Name, d.Id.ToString())).ToListAsync()
                };

            SelectDepartmentModel =
                new SelectDepartmentModel
                {
                    DepartmentId = ApplicationUser.DepartmentId,
                    DepartmentOptions = await db.Departments.Select(d => new SelectListItem(d.Name, d.Id.ToString())).ToListAsync()
                };

            SelectAccountNumberModel =
                new SelectAccountNumberModel
                {
                    AccountNumber = ApplicationUser.AccountNumber ?? string.Empty
                };
            

            if (ApplicationUser.College != null)
            {
                CollegeName = ApplicationUser.College.Name;
            }
            else
            {
                CollegeName = "None";
            }

            if (ApplicationUser.Department != null)
            {
                DepartmentName = ApplicationUser.Department.Name;
            }
            else
            {
                DepartmentName = "None";
            }

            AccountNumber = ApplicationUser.AccountNumber ?? "None"; 

        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadDataAsync();
            return Page(); 
        }
    
        public async Task<IActionResult> OnPostChangeEmailAsync()
        {

            await LoadApplicationUserAsync();

            ModelState.Clear();
            if (!TryValidateModel(ResetEmailModel, nameof(ResetEmailModel)))
            {
                return Page();
            }

            try
            {
                await userManager.SetEmailAsync(ApplicationUser, ResetEmailModel.NewEmail);
                await userManager.SetUserNameAsync(ApplicationUser, ResetEmailModel.NewEmail);
            }
            catch (Exception e)
            {
                Console.WriteLine("##################");
                Console.WriteLine(e.Message);
                Console.WriteLine("##################");
            }

            await LoadDataAsync();
            return Page(); 
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            await LoadApplicationUserAsync();

            ApplicationUser applicationUser = await userManager.GetUserAsync(User) ?? throw new InvalidOperationException("Failed to identify authenticated user.");
            string oldHash = userManager.PasswordHasher.HashPassword(applicationUser, ResetPasswordModel.OldPassword);

            var result = await userManager.ChangePasswordAsync(applicationUser, ResetPasswordModel.OldPassword, ResetPasswordModel.ConfirmNewPassword);

            await LoadDataAsync();

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

        public async Task<IActionResult> OnPostChangeCollegeAsync()
        {
            await LoadApplicationUserAsync();

            // Identify user and college 
            ApplicationUser applicationUser = await userManager.GetUserAsync(User)
                ?? throw new InvalidOperationException("Authenticated user not found.");
            College college = await db.Colleges.FindAsync(SelectCollegeModel.CollegeId)
                ?? throw new InvalidOperationException("Selected college not found.");

            // If user is already a member of this college, skip to department. 
            if (applicationUser.CollegeId == college.Id)
            {
                return RedirectToPage("/Account/Control/Department");
            }

            // Overwrite values 
            // Prevent user from belonging to a department of a different college. 
            applicationUser.DepartmentId = null;
            applicationUser.Department = null;
            applicationUser.CollegeId = college.Id;
            applicationUser.College = college;

            // Save changes 
            var result = await userManager.UpdateAsync(applicationUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(String.Join(", ", result.Errors));
            }


            await LoadDataAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostChangeDepartmentAsync()
        {
            await LoadApplicationUserAsync();

            Department department = await db.Departments.FindAsync(SelectDepartmentModel.DepartmentId) ?? throw new InvalidOperationException("Invalid department selected");

            // Overwrite
            ApplicationUser.DepartmentId = department.Id;
            ApplicationUser.Department = department;

            // Save changes 
            var result = await userManager.UpdateAsync(ApplicationUser);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(String.Join(", ", result.Errors));
            }

            await LoadDataAsync();
            return Page(); 
        }

        public async Task<IActionResult> OnPostChangeAccountNumberAsync()
        {
            await LoadApplicationUserAsync();

            ModelState.Clear();
            
            if ( TryValidateModel(SelectAccountNumberModel) )
            {
                Console.WriteLine("OK, save changes");
                ApplicationUser.AccountNumber = SelectAccountNumberModel.AccountNumber;
                Console.WriteLine(SelectAccountNumberModel.AccountNumber);
                Console.WriteLine(ApplicationUser.AccountNumber);
                await db.SaveChangesAsync();
            }

            await LoadDataAsync();
            return Page(); 
        }
    
    }
}
