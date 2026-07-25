using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Models
{
    public class ResetEmailModel
    {

        [DisplayName("Current Email")]
        [BindProperty]
        [Required]
        public string CurrentEmail { get; set; } = string.Empty;

        [DisplayName("New Email")]
        [BindProperty]
        [Required]
        public string NewEmail { get; set; } = string.Empty;

        [DisplayName("Confirm New Email")]
        [BindProperty]
        [Required]
        [Compare(nameof(NewEmail),
        ErrorMessage = "The email addresses do not match.")]
        public string ConfirmNewEmail { get; set; } = string.Empty;

    }
}
