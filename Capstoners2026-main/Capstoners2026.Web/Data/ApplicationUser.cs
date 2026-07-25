using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Capstoners2026.Web.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? CollegeId { get; set; }
        public int? DepartmentId { get; set; }
        [RegularExpression(@"^[0-9]{6}$")]
        public string? AccountNumber { get; set; }
        public College? College { get; set; }
        public Department? Department { get; set; }
    }
}
