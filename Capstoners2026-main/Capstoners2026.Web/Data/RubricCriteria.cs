using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class RubricCriteria
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int DisplayOrder { get; set; }

    public int RubricId { get; set; }
    public Rubric? Rubric { get; set; }

    public ICollection<RubricScoreOption> ScoreOptions { get; set; } = new List<RubricScoreOption>();
}