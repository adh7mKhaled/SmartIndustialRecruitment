namespace SmartIndustialRecruitment.Contracts.Authentication;

public record RegisterRequest(
    string? PhoneNumber,
    string? Email,
    string Password,
	string FullName,
    bool IsEmail
);