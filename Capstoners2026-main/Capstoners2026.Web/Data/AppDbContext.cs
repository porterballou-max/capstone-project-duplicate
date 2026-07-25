using Capstoners2026.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<College> Colleges { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Grant> Grants { get; set; }
    public DbSet<Rubric> Rubrics { get; set; }
    public DbSet<RubricCriteria> RubricCriteria { get; set; }
    public DbSet<RubricScoreOption> RubricScoreOptions { get; set; }
    public DbSet<BudgetItem> BudgetItems { get; set; }
    public DbSet<AllocationRound> AllocationRounds { get; set; }
    public DbSet<GrantReview> GrantReviews { get; set; }
    public DbSet<GrantReviewScore> GrantReviewScores { get; set; }
    public DbSet<AllocationRule> AllocationRules { get; set; }
    public DbSet<GrantReport> GrantReports { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<College>()
            .HasOne(c => c.Dean)
            .WithMany()
            .HasForeignKey(c => c.DeanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Department>()
            .HasOne(d => d.Chair)
            .WithMany()
            .HasForeignKey(d => d.ChairId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Grant>()
            .HasOne(g => g.ProjectDirector)
            .WithMany()
            .HasForeignKey(g => g.ProjectDirectorId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Grant>()
            .HasOne(g => g.Department)
            .WithMany()
            .HasForeignKey(g => g.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<BudgetItem>()
            .HasOne(b => b.Grant)
            .WithMany(g => g.BudgetItems)
            .HasForeignKey(b => b.GrantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.College)
            .WithMany()
            .HasForeignKey(u => u.CollegeId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Department)
            .WithMany()
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.Entity<Grant>()
            .HasOne(g => g.AllocationRound)
            .WithMany(r => r.Grants)
            .HasForeignKey(g => g.AllocationRoundId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<GrantReview>()
            .HasOne(gr => gr.Grant)
            .WithMany(g => g.Reviews)
            .HasForeignKey(gr => gr.GrantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GrantReview>()
            .HasOne(gr => gr.Reviewer)
            .WithMany()
            .HasForeignKey(gr => gr.ReviewerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GrantReviewScore>()
            .HasOne(grs => grs.GrantReview)
            .WithMany(gr => gr.Scores)
            .HasForeignKey(grs => grs.GrantReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<GrantReviewScore>()
            .HasOne(grs => grs.RubricCriteria)
            .WithMany()
            .HasForeignKey(grs => grs.RubricCriteriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<GrantReport>()
            .HasOne(r => r.Grant)
            .WithMany()
            .HasForeignKey(r => r.GrantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}