using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Models
{
    public class ResetPasswordModel
    {
        [DisplayName("Old Password")]
        [BindProperty]
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [DisplayName("New Password")]
        [BindProperty]
        [Required]
        public string NewPassword { get; set; } = string.Empty;

        [DisplayName("Confirm New Password")]
        [BindProperty]
        [Required]
        [Compare(nameof(NewPassword),
        ErrorMessage = "The passwords do not match.")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
