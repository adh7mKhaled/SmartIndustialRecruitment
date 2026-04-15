using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Entities.Categories;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

        builder.HasMany(x => x.Jobs)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
            
        builder.HasMany(x => x.WorkerSkills)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId);
    }
}
