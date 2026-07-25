using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class RubricScoreOption
{
    public int Id { get; set; }

    public int Value { get; set; }

    public string? Description { get; set; }

    public int RubricCriteriaId { get; set; }
    public RubricCriteria? Criteria { get; set; }
}