using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.JobApplications;

namespace SmartIndustrialRecruitment.Entities.Workers;

public class Worker : ApplicationUser
{
    public string JobTitle { get; set; } = string.Empty;
    public int YearsOfExperience { get; set; }
    public string City { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? HourlyRate { get; set; }
    public bool IsAvailable { get; set; } = true;

    public ICollection<WorkerSkill> Skills { get; set; } = [];
    public ICollection<JobApplication> Applications { get; set; } = [];
}