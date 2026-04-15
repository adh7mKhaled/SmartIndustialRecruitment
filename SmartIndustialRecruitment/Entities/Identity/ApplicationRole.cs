using Microsoft.AspNetCore.Identity;

namespace SmartIndustrialRecruitment.Entities.Identity;

public class ApplicationRole : IdentityRole
{
    public ApplicationRole()
    {
        Id = Guid.CreateVersion7().ToString();
    }

    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }
}