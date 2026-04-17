using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.Workers;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class WorkerConfiguration : IEntityTypeConfiguration<Worker>
{
    public void Configure(EntityTypeBuilder<Worker> builder)
    {
        builder.Property(x => x.JobTitle).HasMaxLength(100);
        builder.Property(x => x.City).HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.HasMany(x => x.Skills)
            .WithOne(x => x.Worker)
            .HasForeignKey(x => x.WorkerId);

        builder.HasMany(x => x.Applications)
            .WithOne(x => x.Worker)
            .HasForeignKey(x => x.WorkerId);

        builder.Property(w => w.HourlyRate)
            .HasPrecision(10, 2);
    }
}