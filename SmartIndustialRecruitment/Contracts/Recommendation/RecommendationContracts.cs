namespace SmartIndustrialRecruitment.Contracts.Recommendation;

// ===========================
// Request من الـ Frontend
// ===========================
public record RecommendationRequest(
    int? WorkerJobType,    // نوع الوظيفة (0-15) - اختياري
    int? WorkerLocation,   // المحافظة (0-26) - اختياري
    int? WorkerExperience  // سنوات الخبرة - اختياري
);

// ===========================
// Response للـ Frontend
// ===========================
public record RecommendationResponse(
    List<JobMatchResult> TopMatches
);

public record JobMatchResult(
    string JobName,               // اسم الوظيفة بالعربي
    int JobLocation,              // رقم المحافظة
    double DistanceKm,            // المسافة بالكيلومتر
    double MatchScore,            // نسبة التطابق (0 - 1)
    int? JobId = null,            // معرف الوظيفة الفعلي في قاعدة البيانات (اختياري)
    string? EmployerName = null,  // اسم صاحب العمل (اختياري)
    string? JobTypeLabel = null   // نوع العمل باللغة العربية (اختياري)
);
