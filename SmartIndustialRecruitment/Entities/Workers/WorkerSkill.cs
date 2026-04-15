using SmartIndustrialRecruitment.Entities.Categories;

namespace SmartIndustrialRecruitment.Entities.Workers;

public class WorkerSkill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string WorkerId { get; set; } = null!;
    public Worker? Worker { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}