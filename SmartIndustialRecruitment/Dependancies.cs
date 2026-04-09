using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SmartIndustialRecruitment.Persistance;

namespace SmartIndustialRecruitment;

public static class Dependancies
{
    public static IServiceCollection AddDependecies(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

        services.AddHttpContextAccessor();
        services.AddSwaggerServices();

        services.AddCors(options =>
            options.AddPolicy("myPolicy", builder =>
                builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()!)
            )
        );

        return services;
    }

    private static IServiceCollection AddSwaggerServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Smart Industial Recruitment",
                Description = "An ASP.NET Core Web API for managing ToDo items"
            });
        });

        return services;
    }
}