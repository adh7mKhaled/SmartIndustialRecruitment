using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Profile;

namespace SmartIndustrialRecruitment.Services.Profile;

public interface IProfileService
{
    Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result> UpdateWorkerProfileAsync(string workerId, UpdateWorkerProfileRequest request, CancellationToken cancellationToken = default);
    Task<Result> UpdateEmployerProfileAsync(string employerId, UpdateEmployerProfileRequest request, CancellationToken cancellationToken = default);
}
