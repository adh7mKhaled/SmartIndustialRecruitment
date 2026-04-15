using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.Jobs;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.City).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Address).HasMaxLength(500).IsRequired();
        builder.Property(x => x.Salary).HasColumnType("decimal(18,2)");

        builder.Property(x => x.JobType)
            .HasConversion<string>();

        builder.Property(x => x.Status)
            .HasConversion<string>();
            
        builder.HasMany(x => x.Applications)
            .WithOne(x => x.Job)
            .HasForeignKey(x => x.JobId);
    }
}
