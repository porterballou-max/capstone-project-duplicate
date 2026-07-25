using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ClosedXML.Excel;

namespace Capstoners2026.Web.Pages.Allocations;

[Authorize(Roles = "Admin,CommitteeChair")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<AllocationRound> Rounds { get; set; } = [];

    [BindProperty]
    public AllocationRound NewRound { get; set; } = new();

    public List<Grant> Grants { get; set; } = [];

    public List<Grant> SurvivingGrants { get; set; } = [];

    public List<Grant> RejectedGrants { get; set; } = [];

    [BindProperty]
    public decimal CutoffPercentage { get; set; }

    public bool CutoffApplied { get; set; } = false;

    [BindProperty]
    public string AllocationRulesJson { get; set; } = "";

    public bool FundingRulesApplied { get; set; } = false;

    public async Task OnGetAsync()
    {
        await LoadRounds();
        await LoadEligibleGrants();

        var activeRound = Rounds.FirstOrDefault(r => r.IsActive);

        if (activeRound != null)
        {
            CutoffPercentage = activeRound.CutoffPercentage;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadRounds();
            await LoadEligibleGrants();
            return Page();
        }

        var previousRound = await _db.AllocationRounds
            .OrderByDescending(r => r.CreatedDate)
            .FirstOrDefaultAsync();

        decimal rollover =
            previousRound == null
                ? 0
                : previousRound.TotalFundsAvailable
                  - previousRound.FundsAllocated;

        // deactivate previous active rounds
        var activeRounds = await _db.AllocationRounds
            .Where(r => r.IsActive)
            .ToListAsync();

        foreach (var round in activeRounds)
        {
            round.IsActive = false;
        }

        NewRound.RolledOverFunds = rollover;
        NewRound.IsActive = true;

        _db.AllocationRounds.Add(NewRound);

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    private async Task LoadRounds()
    {
        Rounds = await _db.AllocationRounds
            .OrderByDescending(r => r.CreatedDate)
            .ToListAsync();
    }

    //Methods for Allocations Cutoff
    private decimal GetGrantScore(Grant grant)
    {
        return grant.Reviews.Any()
            ? grant.Reviews.Average(r => r.FinalPercentage)
            : 0;
    }

    private decimal GetArccRequested(Grant grant)
    {
        return grant.BudgetItems.Sum(b => b.ArccAmount);
    }

    private decimal GetFundingPercentForScore(decimal score, List<AllocationRule> rules)
    {
        var matchingRule = rules.FirstOrDefault(r =>
            score >= r.MinimumScore &&
            score < r.MaximumScore);

        return matchingRule?.FundingPercent ?? 0;
    }

    private async Task LoadEligibleGrants()
    {
        Grants = await _db.Grants
            .Include(g => g.ProjectDirector)
            .Include(g => g.BudgetItems)
            .Include(g => g.Reviews)
            .Where(g =>
                g.IsSubmitted)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostApplyCutoffAsync()
    {
        await LoadRounds();
        await LoadEligibleGrants();

        SurvivingGrants = Grants
            .Where(g => GetGrantScore(g) >= CutoffPercentage)
            .ToList();

        RejectedGrants = Grants
            .Where(g => GetGrantScore(g) < CutoffPercentage)
            .ToList();

        CutoffApplied = true;

        return Page();
    }

    public async Task<IActionResult> OnPostApplyFundingRulesAsync()
    {
        await LoadRounds();
        await LoadEligibleGrants();

        SurvivingGrants = Grants
            .Where(g => GetGrantScore(g) >= CutoffPercentage)
            .ToList();

        RejectedGrants = Grants
            .Where(g => GetGrantScore(g) < CutoffPercentage)
            .ToList();

        var rules = string.IsNullOrWhiteSpace(AllocationRulesJson)
            ? new List<AllocationRule>()
            : JsonSerializer.Deserialize<List<AllocationRule>>(AllocationRulesJson) ?? new List<AllocationRule>();

        foreach (var grant in SurvivingGrants)
        {
            var score = GetGrantScore(grant);
            var fundingPercent = GetFundingPercentForScore(score, rules);
            var arccRequested = GetArccRequested(grant);

            grant.AllocatedAmount = arccRequested * (fundingPercent / 100m);
        }

        CutoffApplied = true;
        FundingRulesApplied = true;

        var activeRound = Rounds.FirstOrDefault(r => r.IsActive);

        if (activeRound == null)
        {
            ModelState.AddModelError("", "There is no active allocation round.");
            return Page();
        }

        var availableFunds = activeRound.TotalFundsAvailable;
        var totalArccRequested = SurvivingGrants.Sum(GetArccRequested);
        var totalAllocated = SurvivingGrants.Sum(g => g.AllocatedAmount ?? 0);

        if (totalAllocated > availableFunds)
        {
            ModelState.AddModelError(
                "",
                $"The proposed allocation total of {totalAllocated:C} exceeds the available funds of {availableFunds:C}. Please adjust the funding rules."
            );
        }

        if (totalArccRequested > availableFunds && totalAllocated < availableFunds)
        {
            ModelState.AddModelError(
                "",
                $"The surviving grants requested {totalArccRequested:C}, but the current rules only allocate {totalAllocated:C} of {availableFunds:C}. Please adjust the rules to use as much available funding as possible."
            );
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostFinalizeAllocationAsync()
    {
        await LoadRounds();
        await LoadEligibleGrants();

        var rules = string.IsNullOrWhiteSpace(AllocationRulesJson)
            ? new List<AllocationRule>()
            : JsonSerializer.Deserialize<List<AllocationRule>>(AllocationRulesJson)
                ?? new List<AllocationRule>();

        var activeRound = Rounds.FirstOrDefault(r => r.IsActive);

        if (activeRound == null)
        {
            ModelState.AddModelError("", "There is no active allocation round.");
            return Page();
        }

        var reportingDate = new DateTime(DateTime.Today.Year + 1, 6, 30);

        foreach (var grant in Grants)
        {
            var score = GetGrantScore(grant);

            if (score < CutoffPercentage)
            {
                grant.AllocatedAmount = 0;
                grant.AllocationFinalized = true;
                grant.AllocationDecision =
                    Grant.AllocationDecisionStatus.Rejected;
                grant.ReportingDueDate = null;

                continue;
            }

            var fundingPercent =
                GetFundingPercentForScore(score, rules);

            var arccRequested =
                GetArccRequested(grant);

            grant.AllocatedAmount =
                arccRequested * (fundingPercent / 100m);

            grant.AllocationFinalized = true;

            if (grant.AllocatedAmount > 0)
            {
                grant.AllocationDecision =
                    Grant.AllocationDecisionStatus.Approved;

                grant.ReportingDueDate = reportingDate;
            }
            else
            {
                grant.AllocationDecision =
                    Grant.AllocationDecisionStatus.Rejected;

                grant.ReportingDueDate = null;
            }
        }

        activeRound.FundsAllocated = Grants.Sum(g => g.AllocatedAmount ?? 0);

        await _db.SaveChangesAsync();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSendToAccountingAsync()
    {
        var activeRound = await _db.AllocationRounds
            .FirstOrDefaultAsync(r => r.IsActive);

        if (activeRound == null)
        {
            ModelState.AddModelError(
                "",
                "There is no active allocation round."
            );

            await LoadRounds();
            await LoadEligibleGrants();

            return Page();
        }

        var finalizedGrants = await _db.Grants
            .Include(g => g.ProjectDirector)
            .Include(g => g.BudgetItems)
            .Where(g =>
                g.AllocationRoundId == activeRound.Id &&
                g.AllocationFinalized)
            .OrderBy(g => g.Title)
            .ToListAsync();

        if (finalizedGrants.Count == 0)
        {
            ModelState.AddModelError(
                "",
                "There are no finalized allocations to send to accounting."
            );

            await LoadRounds();
            await LoadEligibleGrants();

            return Page();
        }

        using var workbook = new XLWorkbook();

        var worksheet = workbook.Worksheets.Add(
            "Final Allocations"
        );

        worksheet.Cell(1, 1).Value = "Principal Investigator";
        worksheet.Cell(1, 2).Value = "Grant Title";
        worksheet.Cell(1, 3).Value = "ARCC Amount Requested";
        worksheet.Cell(1, 4).Value = "Amount Allocated";
        worksheet.Cell(1, 5).Value = "Account Number";
        worksheet.Cell(1, 6).Value = "Status";

        var headerRange = worksheet.Range(1, 1, 1, 6);
        headerRange.Style.Font.Bold = true;

        var row = 2;

        foreach (var grant in finalizedGrants)
        {
            var principalInvestigator =
                grant.ProjectDirector?.UserName
                ?? grant.ProjectDirector?.Email
                ?? "Unknown";

            var arccRequested =
                grant.BudgetItems.Sum(b => b.ArccAmount);

            worksheet.Cell(row, 1).Value =
                principalInvestigator;

            worksheet.Cell(row, 2).Value =
                grant.Title;

            worksheet.Cell(row, 3).Value =
                arccRequested;

            worksheet.Cell(row, 4).Value =
                grant.AllocatedAmount ?? 0;

            worksheet.Cell(row, 5).Value =
                grant.AccountNumber;

            worksheet.Cell(row, 6).Value =
                grant.AllocationDecision.ToString();

            row++;
        }

        worksheet.Column(3).Style.NumberFormat.Format =
            "$#,##0.00";

        worksheet.Column(4).Style.NumberFormat.Format =
            "$#,##0.00";

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();

        workbook.SaveAs(stream);

        var fileName =
            $"Accounting-Allocations-{activeRound.Name}-{DateTime.Today:yyyy-MM-dd}.xlsx";

        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
}