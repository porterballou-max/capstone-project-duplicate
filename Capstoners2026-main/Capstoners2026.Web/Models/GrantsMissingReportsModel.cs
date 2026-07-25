using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.Metadata.Ecma335;

namespace Capstoners2026.Web.Models
{
    public class GrantsMissingReportsModel
    {

        public IList<Grant> Grants30Days { get; set; } = [];
        public IList<Grant> Grants7Days { get; set; } = [];
        public IList<Grant> GrantsNextDay { get; set; } = [];

        public async Task Load(AppDbContext context, IdentityUser user)
        {
            // Fetches all grants tied to the given user. 
            var userGrants = await context.Grants.Where(g => g.ProjectDirectorId == user.Id && g.ReportingDueDate != null).ToArrayAsync();

            if (userGrants.IsNullOrEmpty())
            {
                Console.WriteLine(">>>> FOUND NO GRANTS"); return;
            }

            foreach (Grant grant in userGrants)
            {
                // Check if there is a report for the given grant. 
                // If not, we skip this grant. 
                var reportExistsForGrant = await context.GrantReports.Where(r => r.GrantId == grant.Id).AnyAsync(); 
                if (reportExistsForGrant) { continue;  }

                var difference = grant.ReportingDueDate.GetValueOrDefault().Subtract(DateTime.UtcNow);
                var daysDifference = difference.TotalDays;
                if (daysDifference < 1)
                {
                    GrantsNextDay.Add(grant);
                }
                else if (daysDifference < 7)
                {
                    Grants7Days.Add(grant);
                }
                else if (daysDifference < 31)
                {
                    Grants30Days.Add(grant);
                }
            }

        }

    }
}
