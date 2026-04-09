using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartIndustialRecruitment.Entities;

namespace SmartIndustialRecruitment.Persistance;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
    IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(builder);
    }
}