using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Entities.Workers;

namespace SmartIndustrialRecruitment.Entities.Categories;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<WorkerSkill> WorkerSkills { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
}