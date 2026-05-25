namespace SmartIndustrialRecruitment.Contracts.Profile;

public record UpdateWorkerProfileRequest(
    string FullName,
    string? PhoneNumber,
    string City,
    string JobTitle,
    int YearsOfExperience
);
