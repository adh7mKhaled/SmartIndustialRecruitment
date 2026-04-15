using SmartIndustrialRecruitment.Entities.Jobs;

namespace SmartIndustrialRecruitment.Contracts.Jobs;

public record JobResponse(
    int Id,
    string Title,
    string Description,
    string City,
    string Address,
    decimal? Salary,
    JobType JobType,
    JobStatus Status,
    DateTime? Deadline,
    int CategoryId,
    string CategoryName,
    string EmployerId,
    string? EmployerName
);
