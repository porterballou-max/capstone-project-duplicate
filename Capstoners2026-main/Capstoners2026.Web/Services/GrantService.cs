using Capstoners2026.Web.Data;
using Capstoners2026.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Services
{
    public class GrantService
    {

        public async Task LoadOptions(AppDbContext context, GrantViewModel model)
        {
            var departments = await context.Departments
                .OrderBy(u => u.CollegeId)
                .OrderBy(u => u.Name)
                .ToListAsync()
            ;
            model.DepartmentOptions = new SelectList(
                departments,
                nameof(Department.Id),
                nameof(Department.Name)
            );

            var users = await context.Users
           .OrderBy(u => u.Email)
           .ToListAsync();

            model.UserOptions = new SelectList(
                users,
                nameof(IdentityUser.Id),
                nameof(IdentityUser.Email));
        }

        // Buckets the given user's grants by status for the section tables.
        public async Task<GrantStatusSectionsViewModel> LoadStatusSections(AppDbContext context, string userId)
        {
            var model = new GrantStatusSectionsViewModel();

            var grants = await context.Grants
                .Where(g => g.ProjectDirectorId == userId)
                .Include(g => g.Department)
                .ToListAsync();

            foreach (var grant in grants)
            {
                if (grant.IsRejected)
                    model.Rejected.Add(grant);
                else if (grant.IsApproved)
                    model.Approved.Add(grant);
                else if (grant.IsSubmitted)
                    model.Submitted.Add(grant);
                else
                    model.Drafts.Add(grant);
            }

            var grantIds = grants.Select(g => g.Id).ToList();
            var reportedIds = await context.GrantReports
                .Where(r => grantIds.Contains(r.GrantId))
                .Select(r => r.GrantId)
                .ToListAsync();
            model.ReportedGrantIds = reportedIds.ToHashSet();

            return model;
        }

    }
}
