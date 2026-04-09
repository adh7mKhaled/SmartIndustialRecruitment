using FluentValidation;

namespace SmartIndustialRecruitment.Contracts.Authentication;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
	public LoginRequestValidator()
	{
        RuleFor(x => x.Password).NotEmpty();
	}
}