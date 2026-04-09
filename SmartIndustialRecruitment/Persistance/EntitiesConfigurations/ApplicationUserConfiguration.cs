using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustialRecruitment.Abstractions.Consts;
using SmartIndustialRecruitment.Entities;

namespace SmartIndustialRecruitment.Persistance.EntitiesConfigurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(x => x.FullName).HasMaxLength(100);
        builder
            .OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens")
            .WithOwner()
            .HasForeignKey("UserId");

        var passwordHasher = new PasswordHasher<ApplicationUser>();

        //Default Data
        builder.HasData(new ApplicationUser
        {
            Id = DefaultUsers.AdminId,
            FullName = DefaultUsers.AdminName,
            UserName = DefaultUsers.AdminEmail,
            NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
            Email = DefaultUsers.AdminEmail,
            PhoneNumber = DefaultUsers.AdminPhoneNumber,
            NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
            SecurityStamp = DefaultUsers.AdminSecurityStamp,
            ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
            EmailConfirmed = true,
            PasswordHash = DefaultUsers.AdminPasswordHash
        });
    }
}