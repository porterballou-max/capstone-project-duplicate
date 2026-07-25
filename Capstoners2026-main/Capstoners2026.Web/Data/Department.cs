using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Capstoners2026.Web.Data;

public class Department
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int CollegeId { get; set; }
    public College? College { get; set; }
    [Required]
    public string ChairId { get; set; } = string.Empty;
    public ApplicationUser? Chair { get; set; }
}