using Microsoft.AspNetCore.Identity;

namespace SmartIndustrialRecruitment.Entities.Identity;

public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
        Id = Guid.CreateVersion7().ToString();
        SecurityStamp = Guid.CreateVersion7().ToString();
    }

    public string FullName { get; set; } = string.Empty;
    public bool IsDisabled { get; set; }
    public override string? Email { get; set; }
    public override string? PhoneNumber { get; set; }
    public bool IsDeleted { get; set; } = false;

    public List<RefreshToken> RefreshTokens { get; set; } = [];
}