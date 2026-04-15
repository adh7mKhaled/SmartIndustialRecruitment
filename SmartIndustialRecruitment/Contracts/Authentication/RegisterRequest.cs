namespace SmartIndustrialRecruitment.Contracts.Authentication;

public record RegisterRequest(
    string? PhoneNumber,
    string? Email,
    string Password,
    string FullName,
    bool IsEmail
);

public record WorkerRegisterRequest(
    string? PhoneNumber,
    string? Email,
    string Password,
    string FullName,
    bool IsEmail,
    string JobTitle,
    int YearsOfExperience,
    string City
) : RegisterRequest(PhoneNumber, Email, Password, FullName, IsEmail);

public record EmployerRegisterRequest(
    string? PhoneNumber,
    string? Email,
    string Password,
    string FullName,
    bool IsEmail,
    string? CompanyName,
    string Address
) : RegisterRequest(PhoneNumber, Email, Password, FullName, IsEmail);