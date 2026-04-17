using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Error = SmartIndustrialRecruitment.Abstractions.Error;
using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.Workers;
using SmartIndustrialRecruitment.Entities.Employers;
using SmartIndustrialRecruitment.Abstractions.Consts;
using SmartIndustrialRecruitment.Errors;
using SmartIndustrialRecruitment.Contracts.Authentication;
using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Authentication;

namespace SmartIndustrialRecruitment.Services.Authentication;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider,
    SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string identifier, string password, bool isEmail, CancellationToken cancellationToken = default)
    {
        ApplicationUser? user;

        if (isEmail)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == identifier, cancellationToken);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

            user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == identifier, cancellationToken);
        }

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var result = await _signInManager.PasswordSignInAsync(user, password, false, true);

        if (result.Succeeded)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.FullName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }

        var error = result.IsNotAllowed
            ? UserErrors.EmailNotConfirmed
            : result.IsLockedOut
            ? UserErrors.LockedUser
            : UserErrors.InvalidCredentials;

        return Result.Failure<AuthResponse>(error);
    }

    public async Task<Result> RegisterWorkerAsync(WorkerRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateNewUserAsync(request, cancellationToken);
        if (!validationResult.IsSuccess) return validationResult;

        var worker = new Worker
        {
            Email = request.IsEmail ? request.Email : null,
            UserName = request.IsEmail ? request.Email : request.PhoneNumber,
            PhoneNumber = !request.IsEmail ? request.PhoneNumber : null,
            FullName = request.FullName,
            JobTitle = request.JobTitle,
            YearsOfExperience = request.YearsOfExperience,
            City = request.City
        };

        var result = await _userManager.CreateAsync(worker, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(worker, DefaultRoles.Worker);
            return Result.Success();
        }

        return MapIdentityErrors(result);
    }

    public async Task<Result> RegisterEmployerAsync(EmployerRegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateNewUserAsync(request, cancellationToken);
        if (!validationResult.IsSuccess) return validationResult;

        var employer = new Employer
        {
            Email = request.IsEmail ? request.Email : null,
            UserName = request.IsEmail ? request.Email : request.PhoneNumber,
            PhoneNumber = !request.IsEmail ? request.PhoneNumber : null,
            FullName = request.FullName,
            CompanyName = request.CompanyName,
            Address = request.Address
        };

        var result = await _userManager.CreateAsync(employer, request.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(employer, DefaultRoles.Employer);
            return Result.Success();
        }

        return MapIdentityErrors(result);
    }

    private async Task<Result> ValidateNewUserAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request.IsEmail)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return Result.Failure(UserErrors.EmptyEmail);

            var emailExists = await _userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
            if (emailExists)
                return Result.Failure(UserErrors.DuplicatedEmail);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber))
                return Result.Failure(UserErrors.EmptyPhoneNumber);

            var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber, cancellationToken);
            if (phoneExists)
                return Result.Failure(UserErrors.DuplicatedPhonenumber);
        }
        return Result.Success();
    }

    private Result MapIdentityErrors(IdentityResult result)
    {
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = request.Code;

        try
        {
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        }
        catch (FormatException)
        {
            return Result.Failure(UserErrors.InvalidCode);
        }

        var result = await _userManager.ConfirmEmailAsync(user, code);

        if (result.Succeeded)
        {
            //await _userManager.AddToRoleAsync(user, DefaultRoles.Member);
            return Result.Success();
        }

        return MapIdentityErrors(result);
    }

    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation(message: "Confirmation code: {code}", code);

        return Result.Success();
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        if (user.LockoutEnd > DateTime.UtcNow)
            return Result.Failure<AuthResponse>(UserErrors.LockedUser);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var userRoles = await _userManager.GetRolesAsync(user);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.FullName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> SendResetPasswordCodeAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.Status400BadRequest });

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        _logger.LogInformation(message: "Confirmation code: {code}", code);

        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        IdentityResult result;

        try
        {
            var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
        }
        catch (FormatException)
        {
            result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
        }

        if (result.Succeeded)
            return Result.Success();

        return MapIdentityErrors(result);
    }

    private static string GenerateRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}