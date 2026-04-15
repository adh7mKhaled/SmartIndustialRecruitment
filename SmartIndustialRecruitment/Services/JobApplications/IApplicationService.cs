using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Applications;
using SmartIndustrialRecruitment.Entities.JobApplications;

namespace SmartIndustrialRecruitment.Services.JobApplications;

public interface IApplicationService
{
    Task<Result<ApplicationResponse>> ApplyAsync(string workerId, ApplyForJobRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ApplicationResponse>>> GetWorkerApplicationsAsync(string workerId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ApplicationResponse>>> GetJobApplicationsAsync(int jobId, string employerId, CancellationToken cancellationToken = default);
    Task<Result> UpdateStatusAsync(int applicationId, string employerId, ApplicationStatus status, CancellationToken cancellationToken = default);
}
