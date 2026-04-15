using SmartIndustrialRecruitment.Entities.Jobs;

namespace SmartIndustrialRecruitment.Contracts.Jobs;

public record CreateJobRequest(
    string Title,
    string Description,
    string City,
    string Address,
    decimal? Salary,
    JobType JobType,
    int CategoryId,
    DateTime? Deadline
);
