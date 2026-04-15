using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartIndustrialRecruitment.Abstractions.Consts;
using SmartIndustrialRecruitment.Entities.Identity;

namespace SmartIndustrialRecruitment.Persistance.EntitiesConfigurations;

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
			},
            new ApplicationRole
            {
                Id = DefaultRoles.WorkerRoleId,
                Name = DefaultRoles.Worker,
                NormalizedName = DefaultRoles.Worker.ToUpper(),
                ConcurrencyStamp = DefaultRoles.WorkerConcurrencyStamp,
            },
            new ApplicationRole
            {
                Id = DefaultRoles.EmployerRoleId,
                Name = DefaultRoles.Employer,
                NormalizedName = DefaultRoles.Employer.ToUpper(),
                ConcurrencyStamp = DefaultRoles.EmployerConcurrencyStamp,
            },
        ]);
	}
}