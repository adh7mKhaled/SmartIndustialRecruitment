namespace SmartIndustialRecruitment.Abstractions.Consts;

public static class RegexPatterns
{
	public const string Password = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
    public const string MobileNumber = @"^(00201|\\+201|01)[0-5][0-9]{8}$";
}