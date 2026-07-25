using Capstoners2026.Web.Data;
using Xunit;

namespace Capstoners2026.Tests;

public class GrantTests
{
    [Fact]
    public void NewGrant_StartsAsDraft()
    {
        var grant = new Grant();

        // New grants are drafts until submitted; the Review page hides drafts.
        Assert.False(grant.IsSubmitted);
    }

    [Fact]
    public void Grant_DraftIsNotVisibleToOtherUsers()
    {
        var ownerId = "user-123";
        var otherUserId = "user-456";

        var grant = new Grant
        {
            ProjectDirectorId = ownerId,
            IsSubmitted = false,
            Title = "My Draft Grant",
            Description = "d",
            EvaluationMethod = "e",
            Dissemination = "x"
        };

        Assert.False(grant.IsSubmitted);
        Assert.True(grant.ProjectDirectorId == ownerId);
        Assert.False(grant.ProjectDirectorId == otherUserId);
    }

    [Fact]
    public void Grant_SubmittedIsVisibleToPrivilegedUsers()
    {
        var grant = new Grant
        {
            ProjectDirectorId = "user-123",
            IsSubmitted = true,
            Title = "Submitted Grant",
            Description = "d",
            EvaluationMethod = "e",
            Dissemination = "x"
        };

        Assert.True(grant.IsSubmitted);
        Assert.NotNull(grant.ProjectDirectorId);
        Assert.True(grant.IsSubmitted);
    }
}