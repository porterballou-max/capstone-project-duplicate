using Capstoners2026.Web.Data;

namespace Capstoners2026.Web.Models
{

    // Holds the current user's grants split by where they are in the process.
    // Rendered by Pages/Shared/_GrantStatusSections.cshtml on the Grants page
    // and the home page.
    public class GrantStatusSectionsViewModel
    {

        public List<Grant> Drafts { get; set; } = [];

        public List<Grant> Submitted { get; set; } = [];

        public List<Grant> Approved { get; set; } = [];

        public List<Grant> Rejected { get; set; } = [];

        // Ids of grants that already have a report turned in.
        public HashSet<int> ReportedGrantIds { get; set; } = [];

    }
}
