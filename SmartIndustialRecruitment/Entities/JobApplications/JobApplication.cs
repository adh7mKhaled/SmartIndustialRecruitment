using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Entities.Workers;

namespace SmartIndustrialRecruitment.Entities.JobApplications;

public class JobApplication // طلب التوظيف 
{
    public int Id { get; set; }
    public string? CoverLetter { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public int JobId { get; set; }
    public Job? Job { get; set; }

    public string WorkerId { get; set; } = string.Empty;
    public Worker? Worker { get; set; } 
}