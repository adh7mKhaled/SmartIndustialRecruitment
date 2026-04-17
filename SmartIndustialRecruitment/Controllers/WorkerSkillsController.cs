using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartIndustrialRecruitment.Contracts.Workers;
using SmartIndustrialRecruitment.Services.WorkerSkills;
using System.Security.Claims;

namespace SmartIndustrialRecruitment.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = DefaultRoles.Worker)]
public class WorkerSkillsController(IWorkerSkillService workerSkillService) : ControllerBase
{
    private readonly IWorkerSkillService _workerSkillService = workerSkillService;

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddWorkerSkillRequest request, CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _workerSkillService.AddAsync(workerId, request, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<IActionResult> GetMySkills(CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _workerSkillService.GetByWorkerIdAsync(workerId, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit([FromRoute] int id, [FromBody] AddWorkerSkillRequest request, CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _workerSkillService.UpdateAsync(workerId, id, request, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _workerSkillService.DeleteAsync(workerId, id, cancellationToken);
        
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}