namespace SmartIndustrialRecruitment.Contracts.Authentication;

public record AuthResponse(
	string Id,
	string FullName,
	string Token,
	int ExpiresIn,
	string RefreshToken,
	DateTime RefreshTokenExpiration
);