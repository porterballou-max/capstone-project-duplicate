using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Capstoners2026.Web.Models
{
    public class SelectCollegeModel
    {
        [BindProperty]
        public int? CollegeId { get; set; }
        [BindProperty]
        public List<SelectListItem> CollegeOptions { get; set; }

    }
}
