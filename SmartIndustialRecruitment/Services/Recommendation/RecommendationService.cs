using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Recommendation;
using SmartIndustrialRecruitment.Entities.Jobs;
using SmartIndustrialRecruitment.Entities.Workers;
using SmartIndustrialRecruitment.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartIndustrialRecruitment.Services.Recommendation;

public class RecommendationService : IRecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecommendationService> _logger;
    private readonly ApplicationDbContext _context;
    private readonly string _recommendEndpoint;

    // Mapping Job Title / Category to AI Job Type Code
    private static readonly Dictionary<string, int> JobTypeMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { "سباك", 0 }, { "سباكة", 0 },
        { "كهربائي", 1 }, { "كهرباء", 1 },
        { "نجار", 2 }, { "نجارة", 2 },
        { "نقاش", 3 }, { "دهان", 3 },
        { "عامل محارة", 4 }, { "محارة", 4 },
        { "فني تكييف", 5 }, { "تكييف", 5 },
        { "فني صيانة أجهزة منزلية", 6 }, { "صيانة أجهزة", 6 },
        { "حداد", 7 }, { "حدادة", 7 },
        { "عامل نظافة", 8 }, { "نظافة", 8 },
        { "سائق خاص", 9 }, { "سائق", 9 },
        { "عامل تحميل وتنزيل", 10 }, { "تحميل وتنزيل", 10 },
        { "فني كاميرات مراقبة", 11 }, { "كاميرات مراقبة", 11 },
        { "فني دش", 12 }, { "ستالايت", 12 },
        { "مبيض محارة", 13 },
        { "فني سيراميك", 14 }, { "سيراميك", 14 }
    };

    private static readonly Dictionary<int, string> JobTypeReverseMapping = new()
    {
        { 0, "سباك" }, { 1, "كهربائي" }, { 2, "نجار" }, { 3, "نقاش (دهان)" },
        { 4, "عامل محارة" }, { 5, "فني تكييف" }, { 6, "فني صيانة أجهزة منزلية" },
        { 7, "حداد" }, { 8, "عامل نظافة" }, { 9, "سائق خاص" }, { 10, "عامل تحميل وتنزيل" },
        { 11, "فني كاميرات مراقبة" }, { 12, "فني دش" }, { 13, "مبيض محارة" },
        { 14, "فني سيراميك" }, { 15, "وظيفة إضافية" }
    };

    // Mapping Category ID in SQL Db to AI Job Type Code
    private static readonly Dictionary<int, int> CategoryIdToJobType = new()
    {
        { 6, 0 },  // السباكة -> سباك
        { 5, 1 },  // الكهرباء -> كهربائي
        { 7, 2 },  // النجارة -> نجار
        { 4, 7 },  // اللحام والتشكيل -> حداد
        { 1, 4 },  // البناء والتشييد -> عامل محارة
        { 2, 10 }, // التصنيع -> عامل تحميل وتنزيل
        { 3, 9 },  // الخدمات اللوجستية -> سائق خاص
        { 10, 10 },// المستودعات والتخزين -> عامل تحميل وتنزيل
        { 9, 6 }   // الصيانة والإصلاح -> فني صيانة أجهزة
    };

    // Mapping City names in SQL Db to AI Governorate Code
    private static readonly Dictionary<string, int> CityMapping = new(StringComparer.OrdinalIgnoreCase)
    {
        { "القاهرة", 0 }, { "الإسكندرية", 1 }, { "الدقهلية", 2 }, { "دمياط", 3 },
        { "الفيوم", 4 }, { "الغربية", 5 }, { "الإسماعيلية", 6 }, { "الجيزة", 7 },
        { "المنوفية", 8 }, { "المنيا", 9 }, { "القليوبية", 10 }, { "الوادي الجديد", 11 },
        { "الشرقية", 12 }, { "السويس", 13 }, { "أسوان", 14 }, { "أسيوط", 15 },
        { "بني سويف", 16 }, { "بورسعيد", 17 }, { "جنوب سيناء", 18 }, { "كفر الشيخ", 19 },
        { "مطروح", 20 }, { "الأقصر", 21 }, { "قنا", 22 }, { "شمال سيناء", 23 },
        { "سوهاج", 24 }, { "البحر الأحمر", 25 }, { "البحيرة", 26 }
    };

    private static readonly Dictionary<int, string> CityReverseMapping = new()
    {
        { 0, "القاهرة" }, { 1, "الإسكندرية" }, { 2, "الدقهلية" }, { 3, "دمياط" },
        { 4, "الفيوم" }, { 5, "الغربية" }, { 6, "الإسماعيلية" }, { 7, "الجيزة" },
        { 8, "المنوفية" }, { 9, "المنيا" }, { 10, "القليوبية" }, { 11, "الوادي الجديد" },
        { 12, "الشرقية" }, { 13, "السويس" }, { 14, "أسوان" }, { 15, "أسيوط" },
        { 16, "بني سويف" }, { 17, "بورسعيد" }, { 18, "جنوب سيناء" }, { 19, "كفر الشيخ" },
        { 20, "مطروح" }, { 21, "الأقصر" }, { 22, "قنا" }, { 23, "شمال سيناء" },
        { 24, "سوهاج" }, { 25, "البحر الأحمر" }, { 26, "البحيرة" }
    };

    public RecommendationService(
        HttpClient httpClient,
        ILogger<RecommendationService> logger,
        IConfiguration configuration,
        ApplicationDbContext context)
    {
        _httpClient = httpClient;
        _logger = logger;
        _context = context;

        var baseUrl = configuration["PythonAI:BaseUrl"]?.TrimEnd('/');
        _recommendEndpoint = string.IsNullOrWhiteSpace(baseUrl)
            ? "http://localhost:5000/recommend"
            : $"{baseUrl}/recommend";
    }

    public async Task<Result<RecommendationResponse>> GetRecommendedJobsAsync(string workerId, RecommendationRequest? request)
    {
        // 1. Fetch the worker profile from the database
        var worker = await _context.Workers
            .Include(w => w.Skills)
            .FirstOrDefaultAsync(w => w.Id == workerId);

        if (worker == null)
        {
            return Result.Failure<RecommendationResponse>(
                new Error("Recommendation.WorkerNotFound", "لم يتم العثور على بيانات العامل.", 404));
        }

        // 2. Resolve request parameters (use default mapping from profile if not provided)
        int resolvedJobType = 15; // default "وظيفة إضافية"
        if (request?.WorkerJobType != null)
        {
            resolvedJobType = request.WorkerJobType.Value;
        }
        else
        {
            // Attempt to match job title
            if (!string.IsNullOrEmpty(worker.JobTitle) && JobTypeMapping.TryGetValue(worker.JobTitle, out var jobCode))
            {
                resolvedJobType = jobCode;
            }
            else if (worker.Skills.Any())
            {
                foreach (var skill in worker.Skills)
                {
                    if (JobTypeMapping.TryGetValue(skill.Name, out var skillJobCode))
                    {
                        resolvedJobType = skillJobCode;
                        break;
                    }
                }
            }
        }

        int resolvedLocation = 0; // default Cairo
        if (request?.WorkerLocation != null)
        {
            resolvedLocation = request.WorkerLocation.Value;
        }
        else if (!string.IsNullOrEmpty(worker.City) && CityMapping.TryGetValue(worker.City, out var cityCode))
        {
            resolvedLocation = cityCode;
        }

        int resolvedExperience = request?.WorkerExperience ?? worker.YearsOfExperience;

        _logger.LogInformation(
            "Fetching recommendations for Worker {WorkerId} (JobType: {JobType}, Location: {Location}, Exp: {Exp})",
            workerId, resolvedJobType, resolvedLocation, resolvedExperience);

        // 3. Compute Database-Driven Recommendations (Native Engine)
        var dbRecommendations = await RunNativeRecommendationFallback(worker, resolvedJobType, resolvedLocation, resolvedExperience);

        // 4. Try to fetch Python AI Recommendations
        List<JobMatchResult> pythonRecommendations = new();
        try
        {
            var pythonRequest = new
            {
                worker_job_type = resolvedJobType,
                worker_location = resolvedLocation,
                worker_experience = resolvedExperience
            };

            var response = await _httpClient.PostAsJsonAsync(_recommendEndpoint, pythonRequest);

            if (response.IsSuccessStatusCode)
            {
                var pythonResponse = await response.Content.ReadFromJsonAsync<PythonApiResponse>();
                if (pythonResponse?.TopMatches != null)
                {
                    pythonRecommendations = await MapMatchesToDbJobs(pythonResponse.TopMatches, worker);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to Python AI API. Using native C# recommendations only.");
        }

        // 5. Merge Recommendations
        var finalMatches = new List<JobMatchResult>();

        // Add C# DB-based recommendations (prioritize real jobs in the database)
        foreach (var rec in dbRecommendations)
        {
            if (rec.JobId != null)
            {
                finalMatches.Add(rec);
            }
        }

        // Add mapped Python recommendations
        foreach (var rec in pythonRecommendations)
        {
            if (rec.JobId != null)
            {
                if (!finalMatches.Any(m => m.JobId == rec.JobId))
                {
                    finalMatches.Add(rec);
                }
            }
            else
            {
                // General recommendation (no JobId) - can be added if we have space
                finalMatches.Add(rec);
            }
        }

        // Fill up to 5 items with general recommendations from C# fallback if needed
        if (finalMatches.Count < 5)
        {
            foreach (var rec in dbRecommendations)
            {
                if (rec.JobId == null && finalMatches.Count < 5)
                {
                    finalMatches.Add(rec);
                }
            }
        }

        // Sort by match score and return top 5
        var sortedMatches = finalMatches
            .OrderByDescending(r => r.MatchScore)
            .Take(5)
            .ToList();

        return Result.Success(new RecommendationResponse(sortedMatches));
    }

    /// <summary>
    /// Maps the general recommendations from Python (Job Type and Governorate) to actual open jobs in SQL Server.
    /// </summary>
    private async Task<List<JobMatchResult>> MapMatchesToDbJobs(List<PythonJobMatch> matches, Worker worker)
    {
        var resultList = new List<JobMatchResult>();
        
        // Load all open database jobs to match
        var openJobs = await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Employer)
            .Where(j => j.Status == JobStatus.Open)
            .ToListAsync();

        foreach (var match in matches)
        {
            string recJobName = match.JobName;
            string recCity = CityReverseMapping.TryGetValue(match.JobLocation, out var cName) ? cName : "القاهرة";

            // Find jobs in our DB that match the recommended job type and city
            var matchingJobs = openJobs
                .Where(j => (j.City.Equals(recCity, StringComparison.OrdinalIgnoreCase)) &&
                            (j.Title.Contains(recJobName, StringComparison.OrdinalIgnoreCase) ||
                             (j.Category != null && (j.Category.Name.Contains(recJobName, StringComparison.OrdinalIgnoreCase) || 
                                                     recJobName.Contains(j.Category.Name, StringComparison.OrdinalIgnoreCase)))))
                .ToList();

            if (matchingJobs.Any())
            {
                foreach (var job in matchingJobs)
                {
                    resultList.Add(new JobMatchResult(
                        JobName: job.Title,
                        JobLocation: match.JobLocation,
                        DistanceKm: match.DistanceKm,
                        MatchScore: match.MatchScore,
                        JobId: job.Id,
                        EmployerName: job.Employer?.FullName ?? job.Employer?.CompanyName ?? "شركة غير محددة",
                        JobTypeLabel: GetJobTypeLabelArabic(job.JobType)
                    ));
                }
            }
            else
            {
                // No exact job in DB, add the general recommendations
                resultList.Add(new JobMatchResult(
                    JobName: recJobName,
                    JobLocation: match.JobLocation,
                    DistanceKm: match.DistanceKm,
                    MatchScore: match.MatchScore,
                    JobId: null,
                    EmployerName: null,
                    JobTypeLabel: null
                ));
            }
        }

        // Sort by match score and return top 5
        return resultList.OrderByDescending(r => r.MatchScore).Take(5).ToList();
    }

    /// <summary>
    /// Fallback recommendation logic running directly inside C# SQL queries and calculations.
    /// </summary>
    private async Task<List<JobMatchResult>> RunNativeRecommendationFallback(Worker worker, int jobType, int locationCode, int experience)
    {
        _logger.LogInformation("Running Native C# Recommendation Engine Fallback...");

        var openJobs = await _context.Jobs
            .Include(j => j.Category)
            .Include(j => j.Employer)
            .Where(j => j.Status == JobStatus.Open)
            .ToListAsync();

        var matches = new List<JobMatchResult>();

        string targetJobName = JobTypeReverseMapping.TryGetValue(jobType, out var jName) ? jName : "عام";
        string targetCity = CityReverseMapping.TryGetValue(locationCode, out var cName) ? cName : "القاهرة";

        foreach (var job in openJobs)
        {
            double matchScore = 0.1; // Base score

            // 1. Category / Job Title Match (Up to 0.5)
            if (job.Category != null)
            {
                if (CategoryIdToJobType.TryGetValue(job.CategoryId, out var mappedJobType) && mappedJobType == jobType)
                {
                    matchScore += 0.40;
                }
                else if (job.Category.Name.Contains(targetJobName, StringComparison.OrdinalIgnoreCase) || 
                         targetJobName.Contains(job.Category.Name, StringComparison.OrdinalIgnoreCase))
                {
                    matchScore += 0.35;
                }
            }

            if (job.Title.Contains(targetJobName, StringComparison.OrdinalIgnoreCase))
            {
                matchScore += 0.10;
            }

            // Check if worker has skill matching this job's category
            if (job.Category != null && worker.Skills.Any(s => s.Name.Contains(job.Category.Name, StringComparison.OrdinalIgnoreCase) ||
                                                              job.Category.Name.Contains(s.Name, StringComparison.OrdinalIgnoreCase)))
            {
                matchScore += 0.10;
            }

            // 2. Location Match (Up to 0.3)
            double distanceKm = 150.0; // Default far distance
            if (job.City.Equals(targetCity, StringComparison.OrdinalIgnoreCase))
            {
                matchScore += 0.30;
                distanceKm = 8.4; // Local distance
            }
            else
            {
                // Adjacent governorates get partial score (mock location proximity)
                matchScore += 0.05;
                distanceKm = 85.0;
            }

            // 3. Experience Match (Up to 0.1)
            if (experience >= 5)
            {
                matchScore += 0.10;
            }
            else if (experience >= 2)
            {
                matchScore += 0.07;
            }
            else if (experience > 0)
            {
                matchScore += 0.04;
            }

            // Cap match score at 0.99
            matchScore = Math.Min(0.99, matchScore);

            matches.Add(new JobMatchResult(
                JobName: job.Title,
                JobLocation: CityMapping.TryGetValue(job.City, out var code) ? code : 0,
                DistanceKm: Math.Round(distanceKm, 1),
                MatchScore: Math.Round(matchScore, 3),
                JobId: job.Id,
                EmployerName: job.Employer?.FullName ?? job.Employer?.CompanyName ?? "شركة غير محددة",
                JobTypeLabel: GetJobTypeLabelArabic(job.JobType)
            ));
        }

        // If no jobs match, generate 3 general recommendations so dashboard is not empty
        if (!matches.Any())
        {
            matches.Add(new JobMatchResult(
                JobName: targetJobName,
                JobLocation: locationCode,
                DistanceKm: 0.0,
                MatchScore: 0.850,
                JobId: null,
                EmployerName: null,
                JobTypeLabel: null
            ));
            
            // Add a couple of alternative job types
            var random = new Random();
            for (int i = 0; i < 2; i++)
            {
                int randomJobType = random.Next(0, 15);
                if (randomJobType == jobType) randomJobType = (randomJobType + 1) % 15;

                matches.Add(new JobMatchResult(
                    JobName: JobTypeReverseMapping[randomJobType],
                    JobLocation: locationCode,
                    DistanceKm: 12.0 + (i * 15.5),
                    MatchScore: 0.650 - (i * 0.12),
                    JobId: null,
                    EmployerName: null,
                    JobTypeLabel: null
                ));
            }
        }

        return matches.OrderByDescending(r => r.MatchScore).Take(5).ToList();
    }

    private static string GetJobTypeLabelArabic(JobType type)
    {
        return type switch
        {
            JobType.FullTime => "دوام كامل",
            JobType.PartTime => "دوام جزئي",
            JobType.Contract => "عقد مؤقت",
            JobType.Freelance => "عمل حر",
            _ => "غير محدد"
        };
    }

    // Response classes from Python API
    private class PythonApiResponse
    {
        public List<PythonJobMatch>? TopMatches { get; set; }
    }

    private class PythonJobMatch
    {
        public string JobName { get; set; } = string.Empty;
        public int JobLocation { get; set; }
        public double DistanceKm { get; set; }
        public double MatchScore { get; set; }
    }
}
