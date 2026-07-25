using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class AllocationRound
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public decimal NewFundsAvailable { get; set; }

    public decimal RolledOverFunds { get; set; }

    //Store 70.0 for 70%, will display "70%" to user for better UX
    [Range(0, 100)]
    public decimal CutoffPercentage { get; set; }

    public decimal FundsAllocated { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; }

    public ICollection<Grant> Grants { get; set; }
        = new List<Grant>();

    public decimal TotalFundsAvailable =>
        NewFundsAvailable + RolledOverFunds;

    public decimal RemainingFunds =>
        TotalFundsAvailable - FundsAllocated;

    public ICollection<AllocationRule> AllocationRules { get; set; }
    = new List<AllocationRule>();
}