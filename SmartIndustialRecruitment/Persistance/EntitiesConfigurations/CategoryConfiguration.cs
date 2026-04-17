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

        builder.HasData(
            new Category { Id = 1, Name = "البناء والتشييد" },
            new Category { Id = 2, Name = "التصنيع" },
            new Category { Id = 3, Name = "الخدمات اللوجستية" },
            new Category { Id = 4, Name = "اللحام والتشكيل" },
            new Category { Id = 5, Name = "الكهرباء" },
            new Category { Id = 6, Name = "السباكة" },
            new Category { Id = 7, Name = "النجارة" },
            new Category { Id = 8, Name = "تشغيل المعدات الثقيلة" },
            new Category { Id = 9, Name = "الصيانة والإصلاح" },
            new Category { Id = 10, Name = "المستودعات والتخزين" }
        );
    }
}