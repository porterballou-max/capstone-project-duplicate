using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Capstoners2026.Web.Models
{

    public class GrantViewModel
    {

        [BindProperty]
        public bool IsReadOnly { get; set; } = false; 

        [BindProperty]
        public Grant Grant { get; set; } = new();

        [BindProperty]
        public List<IFormFile> UploadedFiles { get; set; } = new();

        [BindProperty]
        public IFormFile? IrbApprovalFile { get; set; }

        public SelectList? DepartmentOptions { get; set; }

        public SelectList? UserOptions { get; set; }

        [BindProperty]
        public string BudgetItemsJson { get; set; } = "";

        // Set after a draft is saved so the form can confirm it (Save button - Jake).
        public string? StatusMessage { get; set; }

    }
}
