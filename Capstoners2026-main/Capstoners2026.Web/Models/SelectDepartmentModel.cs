using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Capstoners2026.Web.Models
{
    public class SelectDepartmentModel
    {
        [BindProperty]
        [Required]
        public int? DepartmentId { get; set; }
        public List<SelectListItem> DepartmentOptions { get; set; } = [];

    }
}
