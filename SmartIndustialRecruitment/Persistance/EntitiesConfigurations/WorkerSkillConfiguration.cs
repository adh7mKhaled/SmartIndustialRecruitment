using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.Workers;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class WorkerSkillConfiguration : IEntityTypeConfiguration<WorkerSkill>
{
    public void Configure(EntityTypeBuilder<WorkerSkill> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.HasOne(x => x.Worker)
            .WithMany(x => x.Skills)
            .HasForeignKey(x => x.WorkerId);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.WorkerSkills)
            .HasForeignKey(x => x.CategoryId);
    }
}
