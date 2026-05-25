namespace SmartIndustrialRecruitment.Contracts.Profile;

public record UpdateEmployerProfileRequest(
    string FullName,
    string? PhoneNumber,
    string? CompanyName,
    string Address
);
