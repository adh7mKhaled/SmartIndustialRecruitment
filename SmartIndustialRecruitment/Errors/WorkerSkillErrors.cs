using SmartIndustrialRecruitment.Abstractions;

namespace SmartIndustrialRecruitment.Errors;

public record WorkerSkillErrors
{
    public static readonly Error NotFound =
        new("WorkerSkill.NotFound", "لم يتم العثور على المهارة", StatusCodes.Status404NotFound);

    public static readonly Error SkillAlreadyExists =
        new("WorkerSkill.SkillAlreadyExists", "هذه المهارة مضافة بالفعل", StatusCodes.Status409Conflict);
}
