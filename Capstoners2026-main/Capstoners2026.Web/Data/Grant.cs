using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstoners2026.Web.Data;

public class Grant
{
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public string EvaluationMethod { get; set; } = string.Empty;
    [Required]
    public string Dissemination { get; set; } = string.Empty;
    public string? ProjectDirectorId { get; set; }
    public ApplicationUser? ProjectDirector { get; set; }
    // JSON array of relative paths, e.g. ["/uploads/userId/guid_file.pdf"]
    public string? SubmittedFiles { get; set; }
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }
    public string? Justification { get; set; }
    public string? EducationalEnhancement { get; set; }
    public string? StudentImpact { get; set; }
    public int? DepartmentsInvolved { get; set; }
    public bool? UsesHumanOrAnimalSubjects { get; set; }
    public string? IrbApprovalFile { get; set; }
    public string? ProjectTimeline { get; set; }
    public bool IsSubmitted { get; set; } = false;
    public ICollection<BudgetItem> BudgetItems { get; set; }
    = new List<BudgetItem>();
    //New fields for Allocations Tab - Connecting grants to a specific round of funding
    public int? AllocationRoundId { get; set; }
    public AllocationRound? AllocationRound { get; set; }

    public bool HasMatchingFunds { get; set; } = false;
    public decimal? MatchingFundsAmount { get; set; }

    public bool RequiresDeanApproval { get; set; } // We'll need a trigger to make this function properly on the database side. 

    [RegularExpression(@"^[0-9]{6}$")]
    public string AccountNumber { get; set; } = string.Empty;

    public enum ApprovalStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public ApprovalStatus CollegeDeanApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public ApprovalStatus DepartmentChairApprovalStatus { get; set; } = ApprovalStatus.Pending;

    // Set once every required approval has been granted.
    public DateTime? AwardDate { get; set; }

    public bool IsApproved =>
        DepartmentChairApprovalStatus == ApprovalStatus.Approved
        && (!RequiresDeanApproval || CollegeDeanApprovalStatus == ApprovalStatus.Approved);

    public bool IsRejected =>
        DepartmentChairApprovalStatus == ApprovalStatus.Rejected
        || CollegeDeanApprovalStatus == ApprovalStatus.Rejected;


    //New Fields for Allocations Page
    public decimal ReviewerScore { get; set; }

    public decimal? AllocatedAmount { get; set; }

    public bool AllocationFinalized { get; set; }

    public DateTime? ReportingDueDate { get; set; }

    public ICollection<GrantReview> Reviews { get; set; }
    = new List<GrantReview>();


    // Acknowledgements/E-Sign Before Submission
    [NotMapped]
    public bool Acknowledgement1 { get; set; }

    [NotMapped]
    public bool Acknowledgement2 { get; set; }

    [NotMapped]
    public bool Acknowledgement3 { get; set; }

    [NotMapped]
    public bool Acknowledgement4 { get; set; }

    [NotMapped]
    public string? ESignName { get; set; }


    //New Status Field for Allocations "Send to Accounting" button
    public enum AllocationDecisionStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public AllocationDecisionStatus AllocationDecision { get; set; }
        = AllocationDecisionStatus.Pending;
}
