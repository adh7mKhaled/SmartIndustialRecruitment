namespace SmartIndustialRecruitment.Contracts.Authentication;

public record RefreshTokenRequest(
	string Token,
	string RefreshToken
);