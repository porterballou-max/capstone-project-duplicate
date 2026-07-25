using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class Rubric
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<RubricCriteria> Criteria { get; set; } = new List<RubricCriteria>();
}