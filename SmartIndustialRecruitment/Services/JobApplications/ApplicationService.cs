using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Applications;
using SmartIndustrialRecruitment.Entities.JobApplications;
using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Errors;
using SmartIndustrialRecruitment.Persistance;

namespace SmartIndustrialRecruitment.Services.JobApplications;

public class ApplicationService(ApplicationDbContext context) : IApplicationService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<ApplicationResponse>> ApplyAsync(string workerId, ApplyForJobRequest request, CancellationToken cancellationToken = default)
    {
        var alreadyApplied = await _context.Set<JobApplication>()
            .AnyAsync(a => a.JobId == request.JobId && a.WorkerId == workerId, cancellationToken);

        if (alreadyApplied)
            return Result.Failure<ApplicationResponse>(ApplicationErrors.AlreadyApplied);

        var application = new JobApplication
        {
            JobId = request.JobId,
            WorkerId = workerId,
            CoverLetter = request.CoverLetter,
            Status = ApplicationStatus.Pending,
            AppliedAt = DateTime.UtcNow
        };

        _context.Set<JobApplication>().Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(await GetResponseById(application.Id, cancellationToken));
    }

    public async Task<Result<IEnumerable<ApplicationResponse>>> GetWorkerApplicationsAsync(string workerId, CancellationToken cancellationToken = default)
    {
        var applications = await _context.Set<JobApplication>()
            .Include(a => a.Job)
            .Include(a => a.Worker)
            .Where(a => a.WorkerId == workerId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => MapToResponse(a))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<ApplicationResponse>>(applications);
    }

    public async Task<Result<IEnumerable<ApplicationResponse>>> GetJobApplicationsAsync(int jobId, string employerId, CancellationToken cancellationToken = default)
    {
        var jobExists = await _context.Set<Job>()
            .AnyAsync(j => j.Id == jobId && j.EmployerId == employerId, cancellationToken);

        if (!jobExists)
            return Result.Failure<IEnumerable<ApplicationResponse>>(JobErrors.NotFound);

        var applications = await _context.Set<JobApplication>()
            .Include(a => a.Job)
            .Include(a => a.Worker)
            .Where(a => a.JobId == jobId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => MapToResponse(a))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<ApplicationResponse>>(applications);
    }

    public async Task<Result> UpdateStatusAsync(int applicationId, string employerId, ApplicationStatus status, CancellationToken cancellationToken = default)
    {
        var application = await _context.Set<JobApplication>()
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == applicationId && a.Job!.EmployerId == employerId, cancellationToken);

        if (application is null)
            return Result.Failure(ApplicationErrors.NotFound);

        application.Status = status;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task<ApplicationResponse> GetResponseById(int id, CancellationToken cancellationToken)
    {
        var application = await _context.Set<JobApplication>()
            .Include(a => a.Job)
            .Include(a => a.Worker)
            .FirstAsync(a => a.Id == id, cancellationToken);

        return MapToResponse(application);
    }

    private static ApplicationResponse MapToResponse(JobApplication application) =>
        new(
            application.Id,
            application.JobId,
            application.Job?.Title ?? string.Empty,
            application.WorkerId,
            application.Worker?.FullName ?? string.Empty,
            application.CoverLetter,
            application.Status,
            application.AppliedAt
        );
}
