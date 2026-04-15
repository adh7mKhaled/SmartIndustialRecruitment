using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.Jobs;

namespace SmartIndustrialRecruitment.Entities.Employers;

public class Employer : ApplicationUser
{
    public string? CompanyName { get; set; }
    public string Address { get; set; } = null!;

    public ICollection<Job> Jobs { get; set; } = [];
}