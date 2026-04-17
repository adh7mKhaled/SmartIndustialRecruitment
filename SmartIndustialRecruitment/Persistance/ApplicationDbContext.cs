using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Entities.Categories;
using SmartIndustrialRecruitment.Entities.Employers;
using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.JobApplications;
using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Entities.Workers;
using System.Reflection.Emit;

namespace SmartIndustrialRecruitment.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }

    public DbSet<Employer> Employers { get; set; }
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<WorkerSkill> WorkerSkills { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}