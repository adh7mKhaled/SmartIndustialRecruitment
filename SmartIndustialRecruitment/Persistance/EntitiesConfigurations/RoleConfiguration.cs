using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustialRecruitment.Abstractions.Consts;
using SmartIndustialRecruitment.Entities;

namespace SmartIndustialRecruitment.Persistance.EntitiesConfigurations;

public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
	public void Configure(EntityTypeBuilder<ApplicationRole> builder)
	{
		//Default Data
		builder.HasData([
			new ApplicationRole
			{
				Id = DefaultRoles.AdminRoleId,
				Name = DefaultRoles.Admin,
				NormalizedName = DefaultRoles.Admin.ToUpper(),
				ConcurrencyStamp = DefaultRoles.AdminRoleConcurrencyStamp,
			}
		]);
	}
}