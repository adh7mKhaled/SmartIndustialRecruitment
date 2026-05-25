using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Persistance;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace SmartIndustrialRecruitment.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController(ApplicationDbContext context) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Check if worker
        var worker = await _context.Workers.FindAsync(userId);
        if (worker != null)
        {
            return Ok(new
            {
                fullName = worker.FullName,
                email = worker.Email,
                phoneNumber = worker.PhoneNumber,
                city = worker.City,
                jobTitle = worker.JobTitle,
                yearsOfExperience = worker.YearsOfExperience,
                description = worker.Description,
                hourlyRate = worker.HourlyRate,
                isAvailable = worker.IsAvailable,
                role = "Worker"
            });
    }

        // Check if employer
        var employer = await _context.Employers.FindAsync(userId);
        if (employer != null)
        {
            return Ok(new
            {
                fullName = employer.FullName,
                email = employer.Email,
                phoneNumber = employer.PhoneNumber,
                companyName = employer.CompanyName,
                address = employer.Address,
                role = "Employer"
            });
        }

        return NotFound();
    }

    [HttpPut("worker")]
    [Authorize(Roles = DefaultRoles.Worker)]
    public async Task<IActionResult> UpdateWorkerProfile([FromBody] UpdateWorkerProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var worker = await _context.Workers.FindAsync(userId);
        if (worker == null)
            return NotFound();

        worker.FullName = request.FullName;
        worker.PhoneNumber = request.PhoneNumber;
        worker.City = request.City;
        worker.JobTitle = request.JobTitle;
        worker.YearsOfExperience = request.YearsOfExperience;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("employer")]
    [Authorize(Roles = DefaultRoles.Employer)]
    public async Task<IActionResult> UpdateEmployerProfile([FromBody] UpdateEmployerProfileRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var employer = await _context.Employers.FindAsync(userId);
        if (employer == null)
            return NotFound();

        employer.FullName = request.FullName;
        employer.PhoneNumber = request.PhoneNumber;
        employer.CompanyName = request.CompanyName;
        employer.Address = request.Address;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record UpdateWorkerProfileRequest(
    string FullName,
    string PhoneNumber,
    string City,
    string JobTitle,
    int YearsOfExperience
);

public record UpdateEmployerProfileRequest(
    string FullName,
    string PhoneNumber,
    string CompanyName,
    string Address
);
