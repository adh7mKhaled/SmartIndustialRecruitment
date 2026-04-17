namespace SmartIndustrialRecruitment.Contracts.Workers;

public record AddWorkerSkillRequest(
    string Name,
    int CategoryId
);
