using SmartIndustrialRecruitment.Entities.JobApplications;

namespace SmartIndustrialRecruitment.Contracts.Applications;

public record ApplicationResponse(
    int Id,
    int JobId,
    string JobTitle,
    string WorkerId,
    string WorkerName,
    string? CoverLetter,
    ApplicationStatus Status,
    DateTime AppliedAt
);
