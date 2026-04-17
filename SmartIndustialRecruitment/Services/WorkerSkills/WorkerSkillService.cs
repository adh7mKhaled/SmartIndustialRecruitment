using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Contracts.Workers;
using SmartIndustrialRecruitment.Entities.Workers;
using SmartIndustrialRecruitment.Persistance;

namespace SmartIndustrialRecruitment.Services.WorkerSkills;

public class WorkerSkillService(ApplicationDbContext context) : IWorkerSkillService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<WorkerSkillResponse>> AddAsync(string workerId, AddWorkerSkillRequest request, CancellationToken cancellationToken = default)
    {
        var isSkillExists = await _context.WorkerSkills
            .AnyAsync(x => x.WorkerId == workerId && x.Name.ToLower() == request.Name.ToLower() && x.CategoryId == request.CategoryId, cancellationToken);
            
        if (isSkillExists)
            return Result.Failure<WorkerSkillResponse>(WorkerSkillErrors.SkillAlreadyExists);

        var skill = new WorkerSkill
        {
            WorkerId = workerId,
            Name = request.Name,
            CategoryId = request.CategoryId
        };

        _context.WorkerSkills.Add(skill);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(skill).Reference(x => x.Category).LoadAsync(cancellationToken);

        var response = new WorkerSkillResponse(
            skill.Id,
            skill.Name,
            skill.CategoryId,
            skill.Category!.Name
        );

        return Result.Success(response);
    }

    public async Task<Result<IEnumerable<WorkerSkillResponse>>> GetByWorkerIdAsync(string workerId, CancellationToken cancellationToken = default)
    {
        var skills = await _context.WorkerSkills
            .AsNoTracking()
            .Include(x => x.Category)
            .Where(x => x.WorkerId == workerId)
            .Select(x => new WorkerSkillResponse(
                x.Id,
                x.Name,
                x.CategoryId,
                x.Category!.Name
            ))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<WorkerSkillResponse>>(skills);
    }

    public async Task<Result<WorkerSkillResponse>> UpdateAsync(string workerId, int id, AddWorkerSkillRequest request, CancellationToken cancellationToken = default)
    {
        var skill = await _context.WorkerSkills
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id && x.WorkerId == workerId, cancellationToken);

        if (skill is null)
            return Result.Failure<WorkerSkillResponse>(WorkerSkillErrors.NotFound);

        var isSkillExists = await _context.WorkerSkills
            .AnyAsync(x => x.Id != id && x.WorkerId == workerId && x.Name.ToLower() == request.Name.ToLower() && x.CategoryId == request.CategoryId, cancellationToken);

        if (isSkillExists)
            return Result.Failure<WorkerSkillResponse>(WorkerSkillErrors.SkillAlreadyExists);

        skill.Name = request.Name;
        skill.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(cancellationToken);

        if (skill.CategoryId != skill.Category?.Id)
            await _context.Entry(skill).Reference(x => x.Category).LoadAsync(cancellationToken);

        var response = new WorkerSkillResponse(
            skill.Id,
            skill.Name,
            skill.CategoryId,
            skill.Category!.Name
        );

        return Result.Success(response);
    }

    public async Task<Result> DeleteAsync(string workerId, int id, CancellationToken cancellationToken = default)
    {
        var skill = await _context.WorkerSkills
            .FirstOrDefaultAsync(x => x.Id == id && x.WorkerId == workerId, cancellationToken);

        if (skill is null)
            return Result.Failure(WorkerSkillErrors.NotFound);

        _context.WorkerSkills.Remove(skill);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}