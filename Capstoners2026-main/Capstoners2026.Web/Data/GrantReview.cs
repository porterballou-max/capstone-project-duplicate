using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class GrantReview
{
    public int Id { get; set; }

    [Required]
    public int GrantId { get; set; }
    public Grant Grant { get; set; } = null!;

    [Required]
    public string ReviewerId { get; set; } = string.Empty;
    public ApplicationUser Reviewer { get; set; } = null!;

    public decimal FinalPercentage { get; set; }

    public string? Notes { get; set; }

    public DateTime ReviewedDate { get; set; } = DateTime.UtcNow;

    public ICollection<GrantReviewScore> Scores { get; set; } = new List<GrantReviewScore>();
}