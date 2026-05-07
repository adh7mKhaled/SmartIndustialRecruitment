using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartIndustrialRecruitment.Contracts.Profile;
using SmartIndustrialRecruitment.Services.Profile;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartIndustrialRecruitment.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    private readonly IProfileService _profileService = profileService;

    [HttpGet]
    public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        var result = await _profileService.GetProfileAsync(userId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("worker")]
    [Authorize(Roles = DefaultRoles.Worker)]
    public async Task<IActionResult> UpdateWorkerProfile([FromBody] UpdateWorkerProfileRequest request, CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId)) return Unauthorized();

        var result = await _profileService.UpdateWorkerProfileAsync(workerId, request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPut("employer")]
    [Authorize(Roles = DefaultRoles.Employer)]
    public async Task<IActionResult> UpdateEmployerProfile([FromBody] UpdateEmployerProfileRequest request, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId)) return Unauthorized();

        var result = await _profileService.UpdateEmployerProfileAsync(employerId, request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
}
