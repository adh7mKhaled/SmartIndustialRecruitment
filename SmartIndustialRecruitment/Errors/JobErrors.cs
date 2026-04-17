using SmartIndustrialRecruitment.Abstractions;

namespace SmartIndustrialRecruitment.Errors;

public record JobErrors
{
    public static readonly Error NotFound =
        new("Job.NotFound", "لم يتم العثور على الوظيفة", StatusCodes.Status404NotFound);

    public static readonly Error Unauthorized =
        new("Job.Unauthorized", "غير مصرح لك بتعديل هذه الوظيفة", StatusCodes.Status403Forbidden);
}
