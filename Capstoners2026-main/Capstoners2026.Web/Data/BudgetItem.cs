using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Capstoners2026.Web.Data;

public enum BudgetItemType
{
    Hardware,
    Software
}

public class BudgetItem
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public BudgetItemType ItemType { get; set; }

    [Range(0, double.MaxValue)]
    public decimal DepartmentAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal CollegeAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ArccAmount { get; set; }

    [Range(0, double.MaxValue)]
    public decimal OtherAmount { get; set; }

    public int GrantId { get; set; }

    [JsonIgnore]
    public Grant Grant { get; set; } = null!;
}