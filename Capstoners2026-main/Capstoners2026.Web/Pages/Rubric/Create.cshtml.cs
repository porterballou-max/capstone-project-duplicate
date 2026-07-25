using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Authorization;

namespace Capstoners2026.Web.Pages.Rubric
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public List<CriteriaInput> Criteria { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var rubric = new Data.Rubric
            {
                Name = Name
            };

            foreach (var c in Criteria)
            {
                var criteria = new RubricCriteria
                {
                    Title = c.Title,
                    Description = c.Description,
                    DisplayOrder = Criteria.IndexOf(c),
                    ScoreOptions = c.ScoreOptions.Select(s => new RubricScoreOption
                    {
                        Value = s.Value,
                        Description = s.Description
                    }).ToList()
                };

                rubric.Criteria.Add(criteria);
            }

            _context.Rubrics.Add(rubric);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Rubric/Index");
        }

        public class CriteriaInput
        {
            public string Title { get; set; } = string.Empty;
            public string? Description { get; set; }
            public List<ScoreOptionInput> ScoreOptions { get; set; } = new();
        }

        public class ScoreOptionInput
        {
            public int Value { get; set; }
            public string? Description { get; set; }
        }
    }
}