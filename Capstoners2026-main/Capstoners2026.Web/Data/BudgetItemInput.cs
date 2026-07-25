using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class BudgetItemInput
{
    public string Title { get; set; } = string.Empty;
    public BudgetItemType ItemType { get; set; }

    public decimal DepartmentAmount { get; set; }
    public decimal CollegeAmount { get; set; }
    public decimal ArccAmount { get; set; }
    public decimal OtherAmount { get; set; }
}