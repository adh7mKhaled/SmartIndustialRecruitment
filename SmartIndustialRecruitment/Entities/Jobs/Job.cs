using SmartIndustrialRecruitment.Entities.Categories;
using SmartIndustrialRecruitment.Entities.Employers;
using SmartIndustrialRecruitment.Entities.JobApplications;

namespace SmartIndustrialRecruitment.Entities.Jobs;

public class Job
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Address { get; set; } = null!;
    public decimal? Salary { get; set; }
    public JobType JobType { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Open;
    public DateTime? Deadline { get; set; }

    public string EmployerId { get; set; } = null!;
    public Employer? Employer { get; set; }

    public Category? Category { get; set; }
    public int CategoryId { get; set; }

    public ICollection<JobApplication> Applications { get; set; } = [];
}