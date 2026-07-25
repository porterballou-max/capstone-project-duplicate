using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Models
{
    public class SelectAccountNumberModel
    {
        // We use a string here because we want the user to be able to lead with zeroes: "00212", "05821", "00001", etc.
        [BindProperty]
        [Length(6, 6, ErrorMessage = "Account Numbers are always 6 digits in length.")]
        public string AccountNumber { get; set; } = string.Empty;
    }
}
