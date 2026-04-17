using SmartIndustrialRecruitment.Abstractions;

namespace SmartIndustrialRecruitment.Errors;

public record ApplicationErrors
{
    public static readonly Error NotFound =
        new("Application.NotFound", "لم يتم العثور على طلب التوظيف", StatusCodes.Status404NotFound);

    public static readonly Error AlreadyApplied =
        new("Application.AlreadyApplied", "لقد تقدمت لهذه الوظيفة بالفعل", StatusCodes.Status409Conflict);

    public static readonly Error Unauthorized =
        new("Application.Unauthorized", "غير مصرح لك بالوصول لهذا الطلب", StatusCodes.Status403Forbidden);
}
