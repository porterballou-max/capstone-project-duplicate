using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class AllocationRule
{
    public int Id { get; set; }

    public int AllocationRoundId { get; set; }

    public AllocationRound? AllocationRound { get; set; }

    [Range(0, 100)]
    public decimal MinimumScore { get; set; }

    [Range(0, 100)]
    public decimal MaximumScore { get; set; }

    [Range(0, 100)]
    public decimal FundingPercent { get; set; }
}