using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.Employers;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class EmployerConfiguration : IEntityTypeConfiguration<Employer>
{
    public void Configure(EntityTypeBuilder<Employer> builder)
    {
        builder.ToTable("Employers");

        builder.Property(x => x.CompanyName).HasMaxLength(200);
        builder.Property(x => x.Address).HasMaxLength(500);

        builder.HasMany(x => x.Jobs)
            .WithOne(x => x.Employer)
            .HasForeignKey(x => x.EmployerId);
    }
}
