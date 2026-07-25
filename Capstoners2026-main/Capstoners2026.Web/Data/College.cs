using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Capstoners2026.Web.Data;

public class College
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string DeanId { get; set; } = string.Empty;
    public ApplicationUser? Dean { get; set; }
}