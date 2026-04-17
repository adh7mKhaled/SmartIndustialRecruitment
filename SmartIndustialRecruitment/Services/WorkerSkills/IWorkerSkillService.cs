using SmartIndustrialRecruitment.Contracts.Workers;

namespace SmartIndustrialRecruitment.Services.WorkerSkills;

public interface IWorkerSkillService
{
    Task<Result<WorkerSkillResponse>> AddAsync(string workerId, AddWorkerSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<WorkerSkillResponse>>> GetByWorkerIdAsync(string workerId, CancellationToken cancellationToken = default);
    Task<Result<WorkerSkillResponse>> UpdateAsync(string workerId, int id, AddWorkerSkillRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string workerId, int id, CancellationToken cancellationToken = default);
}