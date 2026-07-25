using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Capstoners2026.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Capstoners2026.Web.Pages.Rubric
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int Id { get; set; }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        [BindProperty]
        public List<CriteriaInput> Criteria { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var rubric = await _context.Rubrics
                .Include(r => r.Criteria)
                    .ThenInclude(c => c.ScoreOptions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rubric == null)
                return NotFound();

            Id = rubric.Id;
            Name = rubric.Name;
            Criteria = rubric.Criteria
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new CriteriaInput
                {
                    Title = c.Title,
                    Description = c.Description,
                    ScoreOptions = c.ScoreOptions.Select(s => new ScoreOptionInput
                    {
                        Value = s.Value,
                        Description = s.Description
                    }).ToList()
                }).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var rubric = await _context.Rubrics
                .Include(r => r.Criteria)
                    .ThenInclude(c => c.ScoreOptions)
                .FirstOrDefaultAsync(r => r.Id == Id);

            if (rubric == null)
                return NotFound();

            // Update name
            rubric.Name = Name;

            // Remove old criteria and score options
            _context.RubricCriteria.RemoveRange(rubric.Criteria);

            // Add new criteria and score options
            rubric.Criteria = Criteria.Select((c, i) => new RubricCriteria
            {
                Title = c.Title,
                Description = c.Description,
                DisplayOrder = i,
                ScoreOptions = c.ScoreOptions.Select(s => new RubricScoreOption
                {
                    Value = s.Value,
                    Description = s.Description
                }).ToList()
            }).ToList();

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