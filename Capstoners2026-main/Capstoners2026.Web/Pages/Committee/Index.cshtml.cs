using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Capstoners2026.Web.Pages.Committee;

[Authorize(Roles = "Admin")]
public class CommitteeMembersManagerModel(AppDbContext db, UserManager<ApplicationUser> userManager, ILogger<CommitteeMembersManagerModel> logger) : PageModel
{

    public UserManager<ApplicationUser> UserManager => userManager; 
    const string committeeRole = "Committee";
    const string committeeChairRole = "CommitteeChair"; 
    public List<ApplicationUser> CommitteeMembers { get; set; } = [];
    public List<ApplicationUser> NonCommitteeMembers { get; set; } = [];

    private async Task PopulateAsync()
    {
        var users = await db.Users.ToListAsync();
        foreach (var user in users)
        {
            var isCommitteeMember = await userManager.IsInRoleAsync(user, "Committee");
            if (isCommitteeMember)
            {
                CommitteeMembers.Add(user);
            }
            else
            {
                NonCommitteeMembers.Add(user);
            }
        }
    }

    public async Task OnGetAsync()
    {
        await PopulateAsync();
    }

    public async Task<IActionResult> OnPostRemoveCommitteeMember(string id)
    {
        var user = await db.Users.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NullReferenceException("Tried to operate on nonexistent user.");

        logger.LogInformation("Remove committee member pending...");
        try
        {
            // Remove from committee
            await userManager.RemoveFromRoleAsync(user, committeeRole);
            // Also remove from chair 
            if (await userManager.IsInRoleAsync(user, committeeChairRole))
            {
                await userManager.RemoveFromRoleAsync(user, committeeChairRole); 
            }

        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }

        await PopulateAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAddCommitteeMember(string id)
    {
        var user = await db.Users.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NullReferenceException("Tried to operate on nonexistent user.");

        logger.LogInformation("Add committee member pending...");
        try
        {
            await userManager.AddToRoleAsync(user, committeeRole);
        }
        catch (Exception e)
        {
            ModelState.TryAddModelException("", e); 
        }

        await PopulateAsync(); 
        return Page();

    }


    public async Task<IActionResult> OnPostAddChairRoleToUser(string id)
    {

        // Ensure we can identify the user of given id before making changes to the database
        var targetUser = await db.Users.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NullReferenceException("Tried to operate on nonexistent user.");

        try
        {
            // Ensure only ONE user has this role by removing it from all other users. 
            IList<ApplicationUser> users = await userManager.GetUsersInRoleAsync(committeeChairRole);
            foreach (ApplicationUser user in users)
            {
                await userManager.RemoveFromRoleAsync(user, committeeChairRole);
            }

            // Give chair role to selected user 
            await userManager.AddToRoleAsync(targetUser, committeeChairRole);
        }
        catch (Exception e)
        {
            ModelState.TryAddModelException("", e); 
        }
       

        await PopulateAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostRemoveChairRoleFromUser(string id)
    {
        // Take away role 
        var user = await db.Users.FirstOrDefaultAsync(c => c.Id == id) ?? throw new NullReferenceException("Tried to operate on nonexistent user.");

        try
        {
            await userManager.RemoveFromRoleAsync(user, committeeChairRole);
        }
        catch (Exception e)
        {
            ModelState.TryAddModelException("", e); 
        }

        await PopulateAsync();
        return Page();
    }

}
