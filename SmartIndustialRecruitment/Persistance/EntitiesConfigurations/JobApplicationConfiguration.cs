using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.JobApplications;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.CoverLetter).HasMaxLength(1000);
        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.HasOne(x => x.Job)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Worker)
            .WithMany(x => x.Applications)
            .HasForeignKey(x => x.WorkerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
