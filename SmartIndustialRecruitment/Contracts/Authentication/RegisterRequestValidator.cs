namespace SmartIndustialRecruitment.Contracts.Authentication;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
	public RegisterRequestValidator()
	{
		RuleFor(x => x.PhoneNumber)
			.NotEmpty()
            .Matches(RegexPatterns.MobileNumber)
			.When(x => !string.IsNullOrEmpty(x.PhoneNumber))
			.WithMessage(GlobalErrors.InValideMobileNumber);

        RuleFor(x => x.Email)
			.NotEmpty()
			.When(x => !string.IsNullOrEmpty(x.Email))
			.WithMessage(GlobalErrors.InValideEmail);

        RuleFor(x => x.Password)
			.NotEmpty()
			.Matches(RegexPatterns.Password)
			.WithMessage("Password should be at least 8 digits and should contains Lowercase, NonAlphanumeric and Uppercase");

		RuleFor(x => x.FullName).NotEmpty().Length(3, 100);
    }
}