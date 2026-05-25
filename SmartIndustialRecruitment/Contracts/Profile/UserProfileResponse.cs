namespace SmartIndustrialRecruitment.Contracts.Profile;

public record UserProfileResponse(
    string Id,
    string FullName,
    string? Email,
    string? PhoneNumber,
    string Role,
    
    // Worker specific
    string? City,
    string? JobTitle,
    int? YearsOfExperience,
    
    // Employer specific
    string? CompanyName,
    string? Address
);
