using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Profile;
using SmartIndustrialRecruitment.Entities.Employers;
using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.Workers;

namespace SmartIndustrialRecruitment.Services.Profile;

public class ProfileService(UserManager<ApplicationUser> userManager) : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user is null)
            return Result.Failure<UserProfileResponse>(new Error("User.NotFound", "User not found", StatusCodes.Status404NotFound));

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Unknown";

        if (user is Worker worker)
        {
            return Result.Success(new UserProfileResponse(
                worker.Id,
                worker.FullName,
                worker.Email,
                worker.PhoneNumber,
                role,
                worker.City,
                worker.JobTitle,
                worker.YearsOfExperience,
                null,
                null
            ));
        }

        if (user is Employer employer)
        {
            return Result.Success(new UserProfileResponse(
                employer.Id,
                employer.FullName,
                employer.Email,
                employer.PhoneNumber,
                role,
                null,
                null,
                null,
                employer.CompanyName,
                employer.Address
            ));
        }

        return Result.Success(new UserProfileResponse(
            user.Id,
            user.FullName,
            user.Email,
            user.PhoneNumber,
            role,
            null, null, null, null, null
        ));
    }

    public async Task<Result> UpdateWorkerProfileAsync(string workerId, UpdateWorkerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == workerId, cancellationToken);
        if (user is not Worker worker)
            return Result.Failure(new Error("Worker.NotFound", "Worker not found", StatusCodes.Status404NotFound));

        worker.FullName = request.FullName;
        worker.PhoneNumber = request.PhoneNumber;
        worker.City = request.City;
        worker.JobTitle = request.JobTitle;
        worker.YearsOfExperience = request.YearsOfExperience;
        var result = await _userManager.UpdateAsync(worker);
        return result.Succeeded ? Result.Success() : Result.Failure(new Error("User.UpdateFailed", string.Join(", ", result.Errors.Select(e => e.Description)), StatusCodes.Status400BadRequest));
    }

    public async Task<Result> UpdateEmployerProfileAsync(string employerId, UpdateEmployerProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == employerId, cancellationToken);
        if (user is not Employer employer)
            return Result.Failure(new Error("Employer.NotFound", "Employer not found", StatusCodes.Status404NotFound));

        employer.FullName = request.FullName;
        employer.PhoneNumber = request.PhoneNumber;
        employer.CompanyName = request.CompanyName;
        employer.Address = request.Address;
        var result = await _userManager.UpdateAsync(employer);
        return result.Succeeded ? Result.Success() : Result.Failure(new Error("User.UpdateFailed", string.Join(", ", result.Errors.Select(e => e.Description)), StatusCodes.Status400BadRequest));
    }
}
