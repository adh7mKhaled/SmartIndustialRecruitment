using Microsoft.AspNetCore.Identity;
using SmartIndustrialRecruitment.Abstractions.Consts;
using SmartIndustrialRecruitment.Entities.Employers;
using SmartIndustrialRecruitment.Entities.Identity;
using SmartIndustrialRecruitment.Entities.Jobs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartIndustrialRecruitment.Persistance;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        // 1. Ensure Categories exist
        if (!context.Categories.Any())
        {
            return;
        }

        // 2. Check if we already have any jobs
        if (context.Jobs.Any())
        {
            return;
        }

        // 3. Create a test Employer
        var employerEmail = "employer@test.com";
        var employerUser = await userManager.FindByEmailAsync(employerEmail) as Employer;
        if (employerUser == null)
        {
            employerUser = new Employer
            {
                Email = employerEmail,
                UserName = employerEmail,
                EmailConfirmed = true,
                FullName = "المقاولون العرب (فرع القاهرة)",
                CompanyName = "المقاولون العرب",
                Address = "القاهرة"
            };

            var result = await userManager.CreateAsync(employerUser, "Password123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(employerUser, DefaultRoles.Employer);
            }
        }

        // 4. Create mock Jobs
        var mockJobs = new List<Job>
        {
            new Job
            {
                Title = "سباك صيانة شبكات مياه",
                Description = "مطلوب سباك محترف لديه خبرة لا تقل عن 3 سنوات في صيانة وتمديد شبكات المياه للمباني السكنية.",
                City = "القاهرة",
                Address = "وسط البلد، القاهرة",
                Salary = 6500,
                JobType = JobType.FullTime,
                Status = JobStatus.Open,
                EmployerId = employerUser.Id,
                CategoryId = 6 // السباكة
            },
            new Job
            {
                Title = "فني تمديدات كهربائية",
                Description = "مطلوب كهربائي للعمل في مشروع تشييد عمارات سكنية جديدة بمدينة القاهرة الجديدة.",
                City = "القاهرة",
                Address = "التجمع الخامس، القاهرة",
                Salary = 7000,
                JobType = JobType.FullTime,
                Status = JobStatus.Open,
                EmployerId = employerUser.Id,
                CategoryId = 5 // الكهرباء
            },
            new Job
            {
                Title = "نجار أبواب وديكور خشب",
                Description = "مطلوب نجار لورشة تصنيع أبواب وغرف نوم خشبية بجودة عالية.",
                City = "القاهرة",
                Address = "مصر الجديدة، القاهرة",
                Salary = 6000,
                JobType = JobType.FullTime,
                Status = JobStatus.Open,
                EmployerId = employerUser.Id,
                CategoryId = 7 // النجارة
            },
            new Job
            {
                Title = "حداد أبواب ونوافذ حديدية",
                Description = "مطلوب حداد تشكيل حديد للعمل في مصنع حدادة بالإسكندرية.",
                City = "الإسكندرية",
                Address = "سموحة، الإسكندرية",
                Salary = 5500,
                JobType = JobType.FullTime,
                Status = JobStatus.Open,
                EmployerId = employerUser.Id,
                CategoryId = 4 // اللحام والتشكيل
            },
            new Job
            {
                Title = "عامل بناء ومحارة",
                Description = "مطلوب عمال للعمل في ورشة تشطيبات وبناء بمدينة أسيوط.",
                City = "أسيوط",
                Address = "شارع الجمهورية، أسيوط",
                Salary = 4500,
                JobType = JobType.PartTime,
                Status = JobStatus.Open,
                EmployerId = employerUser.Id,
                CategoryId = 1 // البناء والتشييد
            }
        };

        context.Jobs.AddRange(mockJobs);
        await context.SaveChangesAsync();
    }
}
