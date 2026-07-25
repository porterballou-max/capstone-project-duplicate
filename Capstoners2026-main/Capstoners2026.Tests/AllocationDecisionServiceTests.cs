using Capstoners2026.Web.Data;
using Capstoners2026.Web.Services;
using Xunit;

namespace Capstoners2026.Tests;

public class AllocationDecisionServiceTests
{
    [Fact]
    public void GrantWithAtLeastOneDollarAllocated_IsApproved()
    {
        var grant = new Grant
        {
            AllocatedAmount = 1.00m
        };

        AllocationDecisionService.ApplyDecision(grant);

        Assert.Equal(
            Grant.AllocationDecisionStatus.Approved,
            grant.AllocationDecision);
    }

    [Fact]
    public void GrantWithNoMoneyAllocated_IsRejected()
    {
        var grant = new Grant
        {
            AllocatedAmount = 0m
        };

        AllocationDecisionService.ApplyDecision(grant);

        Assert.Equal(
            Grant.AllocationDecisionStatus.Rejected,
            grant.AllocationDecision);
    }
}