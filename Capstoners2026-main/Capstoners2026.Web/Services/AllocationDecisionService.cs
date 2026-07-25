using Capstoners2026.Web.Data;

namespace Capstoners2026.Web.Services;

public static class AllocationDecisionService
{
    public static void ApplyDecision(Grant grant)
    {
        if (grant.AllocatedAmount > 0)
        {
            grant.AllocationDecision =
                Grant.AllocationDecisionStatus.Approved;
        }
        else
        {
            grant.AllocationDecision =
                Grant.AllocationDecisionStatus.Rejected;
        }
    }
}