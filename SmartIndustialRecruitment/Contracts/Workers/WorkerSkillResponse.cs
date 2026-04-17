namespace SmartIndustrialRecruitment.Contracts.Workers;

public record WorkerSkillResponse(
    int Id,
    string Name,
    int CategoryId,
    string CategoryName
);