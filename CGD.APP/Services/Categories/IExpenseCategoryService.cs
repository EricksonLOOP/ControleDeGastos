using CGD.APP.DTOs.Category;

namespace CGD.APP.Services.Categories;

public interface IExpenseCategoryService
{
    Task<CategoryDto> CreateAsync(Guid userId, CategoryCreateDto dto);
    Task<CategoryDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<CategoryDto>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<CategoryDto>> GetPagedByUserIdAsync(Guid userId, int page, int pageSize);
    Task<CategoryDto> UpdateAsync(Guid id, CategoryCreateDto dto);
    Task DeleteAsync(Guid id);
    Task<CategoryTotalsResponseDto> GetCategoryTotalsAsync(Guid userId);
}
