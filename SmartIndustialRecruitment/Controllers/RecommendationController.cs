using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartIndustrialRecruitment.Abstractions.Consts;
using SmartIndustrialRecruitment.Contracts.Recommendation;
using SmartIndustrialRecruitment.Services.Recommendation;
using System.Security.Claims;

namespace SmartIndustrialRecruitment.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = DefaultRoles.Worker)]
public class RecommendationController(IRecommendationService recommendationService) : ControllerBase
{
    private readonly IRecommendationService _recommendationService = recommendationService;

    /// <summary>
    /// يرسل بيانات العامل ويرجع أحسن 5 وظائف مناسبة
    /// POST /api/recommendation/jobs
    /// </summary>
    [HttpPost("jobs")]
    public async Task<IActionResult> GetRecommendedJobs([FromBody] RecommendationRequest? request)
    {
        var workerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(workerId))
            return Unauthorized();

        var result = await _recommendationService.GetRecommendedJobsAsync(workerId, request);

        if (!result.IsSuccess)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }
}
