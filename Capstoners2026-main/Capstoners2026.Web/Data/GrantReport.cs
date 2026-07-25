using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Data;

public class GrantReport
{
    public int Id { get; set; }

    public int GrantId { get; set; }
    public Grant? Grant { get; set; }

    [Required]
    public string ProjectDirector { get; set; } = string.Empty;

    [Required]
    public string ProjectTitle { get; set; } = string.Empty;

    // Kept as text so the form stays a plain textbox per the requirements.
    public string? AwardDate { get; set; }

    [Required]
    public string ProjectSummary { get; set; } = string.Empty;

    [Required]
    public string CurrentProgress { get; set; } = string.Empty;

    [Required]
    public string NextSteps { get; set; } = string.Empty;

    [Required]
    public string Budget { get; set; } = string.Empty;

    // Relative path of the single uploaded pdf, e.g. "/uploads/userId/guid_report.pdf"
    public string? ReportFile { get; set; }

    public DateTime SubmittedAt { get; set; }
}
