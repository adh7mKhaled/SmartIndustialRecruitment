using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Jobs;

namespace SmartIndustrialRecruitment.Services.Jobs;

public interface IJobService
{
    Task<Result<JobResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PaginatedList<JobResponse>>> GetAllAsync(int pageNumber, int pageSize, int? categoryId, string? city, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<JobResponse>>> GetMyJobsAsync(string employerId, CancellationToken cancellationToken = default);
    Task<Result<JobResponse>> CreateAsync(string employerId, CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateAsync(int id, string employerId, CreateJobRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, string employerId, CancellationToken cancellationToken = default);
}