using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartIndustrialRecruitment.Contracts.Applications;
using SmartIndustrialRecruitment.Entities.JobApplications;
using SmartIndustrialRecruitment.Services.JobApplications;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartIndustrialRecruitment.Controllers;

[Route("api/[controller]")]
[ApiController]
public class JobApplicationsController(IApplicationService applicationService) : ControllerBase
{
    private readonly IApplicationService _applicationService = applicationService;

    [HttpPost("apply")]
    [Authorize(Roles = DefaultRoles.Worker)]
    public async Task<IActionResult> Apply([FromBody] ApplyForJobRequest request, CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _applicationService.ApplyAsync(workerId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my-applications")]
    [Authorize(Roles = DefaultRoles.Worker)]
    public async Task<IActionResult> GetMyApplications(CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _applicationService.GetWorkerApplicationsAsync(workerId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("job/{jobId}")]
    [Authorize(Roles = DefaultRoles.Employer)]
    public async Task<IActionResult> GetJobApplications([FromRoute] int jobId, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _applicationService.GetJobApplicationsAsync(jobId, employerId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPatch("{applicationId}/status")]
    [Authorize(Roles = DefaultRoles.Employer)]
    public async Task<IActionResult> UpdateStatus([FromRoute] int applicationId, [FromBody] ApplicationStatus status, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _applicationService.UpdateStatusAsync(applicationId, employerId, status, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}