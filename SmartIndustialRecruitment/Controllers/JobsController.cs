using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartIndustrialRecruitment.Contracts.Jobs;
using SmartIndustrialRecruitment.Services.Jobs;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartIndustrialRecruitment.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Employer)]
public class JobsController(IJobService jobService) : ControllerBase
{
    private readonly IJobService _jobService = jobService;

    [HttpGet]
    [Authorize(Roles = $"{DefaultRoles.Worker},{DefaultRoles.Employer}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] int? categoryId = null, [FromQuery] string? city = null, CancellationToken cancellationToken = default)
    {
        var result = await _jobService.GetAllAsync(pageNumber, pageSize, categoryId, city, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{DefaultRoles.Worker},{DefaultRoles.Employer}")]
    public async Task<IActionResult> GetById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var result = await _jobService.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("my-jobs")]
    public async Task<IActionResult> GetMyJobs(CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _jobService.GetMyJobsAsync(employerId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _jobService.CreateAsync(employerId, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CreateJobRequest request, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _jobService.UpdateAsync(id, employerId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var employerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(employerId))
            return Unauthorized();

        var result = await _jobService.DeleteAsync(id, employerId, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}