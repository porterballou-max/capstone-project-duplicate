using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class GrantReviewScore
{
    public int Id { get; set; }

    [Required]
    public int GrantReviewId { get; set; }
    public GrantReview GrantReview { get; set; } = null!;

    [Required]
    public int RubricCriteriaId { get; set; }
    public RubricCriteria RubricCriteria { get; set; } = null!;

    [Required]
    public int PointsAwarded { get; set; }

    public int MaxPoints { get; set; }
}