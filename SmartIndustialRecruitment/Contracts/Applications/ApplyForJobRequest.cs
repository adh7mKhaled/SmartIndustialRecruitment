namespace SmartIndustrialRecruitment.Contracts.Applications;

public record ApplyForJobRequest(
    int JobId,
    string? CoverLetter
);
