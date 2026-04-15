using Microsoft.EntityFrameworkCore;
using SmartIndustrialRecruitment.Abstractions;
using SmartIndustrialRecruitment.Contracts.Categories;
using SmartIndustrialRecruitment.Entities.Categories;
using SmartIndustrialRecruitment.Persistance;

namespace SmartIndustrialRecruitment.Services;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
}

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Set<Category>()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<CategoryResponse>>(categories);
    }
}
