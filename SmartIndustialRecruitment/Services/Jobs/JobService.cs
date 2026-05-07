using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Contracts.Jobs;
using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Persistance;

namespace SmartIndustrialRecruitment.Services.Jobs;

public class JobService(ApplicationDbContext context) : IJobService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<JobResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Employer)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);

        if (job is null)
            return Result.Failure<JobResponse>(JobErrors.NotFound);

        return Result.Success(MapToResponse(job));
    }

    public async Task<Result<PaginatedList<JobResponse>>> GetAllAsync(int pageNumber, int pageSize, int? categoryId, string? city, CancellationToken cancellationToken = default)
    {
        var query = _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Employer)
            .AsQueryable();

        if (categoryId.HasValue)
            query = query.Where(j => j.CategoryId == categoryId.Value);

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(j => j.City.Contains(city));

        var jobs = await query
            .OrderByDescending(j => j.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(j => MapToResponse(j))
            .ToListAsync(cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);

        return Result.Success(new PaginatedList<JobResponse>(jobs, pageNumber, totalCount, pageSize));
    }

    public async Task<Result<IEnumerable<JobResponse>>> GetMyJobsAsync(string employerId, CancellationToken cancellationToken = default)
    {
        var jobs = await _context.Jobs
            .Include(j => j.Category)
            .Where(j => j.EmployerId == employerId)
            .OrderByDescending(j => j.Id)
            .Select(j => MapToResponse(j))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<JobResponse>>(jobs);
    }

    public async Task<Result<JobResponse>> CreateAsync(string employerId, CreateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = new Job
        {
            Title = request.Title,
            Description = request.Description,
            City = request.City,
            Address = request.Address,
            Salary = request.Salary,
            JobType = request.JobType,
            CategoryId = request.CategoryId,
            Deadline = request.Deadline,
            EmployerId = employerId,
            Status = JobStatus.Open
        };

        await _context.Jobs.AddAsync(job, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(job.Id, cancellationToken);
    }

    public async Task<Result> UpdateAsync(int id, string employerId, CreateJobRequest request, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employerId, cancellationToken);

        if (job is null)
            return Result.Failure(JobErrors.NotFound);

        job.Title = request.Title;
        job.Description = request.Description;
        job.City = request.City;
        job.Address = request.Address;
        job.Salary = request.Salary;
        job.JobType = request.JobType;
        job.CategoryId = request.CategoryId;
        job.Deadline = request.Deadline;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(int id, string employerId, CancellationToken cancellationToken = default)
    {
        var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == id && j.EmployerId == employerId, cancellationToken);

        if (job is null)
            return Result.Failure(JobErrors.NotFound);

        _context.Set<Job>().Remove(job);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static JobResponse MapToResponse(Job job) =>
        new(
            job.Id,
            job.Title,
            job.Description,
            job.City,
            job.Address,
            job.Salary,
            job.JobType,
            job.Status,
            job.Deadline,
            job.CategoryId,
            job.Category?.Name ?? string.Empty,
            job.EmployerId,
            job.Employer?.FullName
        );
}