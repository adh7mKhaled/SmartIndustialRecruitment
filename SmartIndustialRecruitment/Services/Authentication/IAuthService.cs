using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Authentication;

namespace SmartIndustrialRecruitment.Services.Authentication;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(string identifier, string password, bool isEmail, CancellationToken cancellationToken = default);
    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<Result> RegisterWorkerAsync(WorkerRegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> RegisterEmployerAsync(EmployerRegisterRequest request, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
    Task<Result> SendResetPasswordCodeAsync(string email);
    Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
}