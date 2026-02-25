using CGD.APP.DTOs.Category;
using CGD.APP.Mappers;
using CGD.CrossCutting.Exceptions;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;

namespace CGD.APP.Services.Categories;

public class ExpenseCategoryService(
    IExpenseCategoryRepository categoryRepository,
    IExpenseRepository expenseRepository) : IExpenseCategoryService
{
    private readonly IExpenseCategoryRepository _categoryRepository = categoryRepository;
    private readonly IExpenseRepository _expenseRepository = expenseRepository;

    public async Task<CategoryDto> CreateAsync(Guid userId, CategoryCreateDto dto)
    {
        var category = CategoryMapper.ToEntity(userId, dto);
        await _categoryRepository.AddAsync(category);
        return CategoryMapper.ToDto(category);
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
            throw new ArgumentException("Categoria não encontrada");

        return CategoryMapper.ToDto(category);
    }

    public async Task<IReadOnlyList<CategoryDto>> GetByUserIdAsync(Guid userId)
    {
        var categories = await _categoryRepository.GetByUserIdAsync(userId);
        return categories.Select(CategoryMapper.ToDto).ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetPagedByUserIdAsync(Guid userId, int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentException("Page deve ser maior que zero.");
        if (pageSize <= 0 || pageSize > 100)
            throw new ArgumentException("PageSize deve estar entre 1 e 100.");

        var categories = await _categoryRepository.GetPagedByUserIdAsync(userId, page, pageSize);
        return categories.Select(CategoryMapper.ToDto).ToList();
    }

    public async Task<CategoryDto> UpdateAsync(Guid id, CategoryCreateDto dto)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
            throw new ArgumentException("Categoria não encontrada");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.Purpose = dto.Purpose;

        await _categoryRepository.UpdateAsync(category);
        return CategoryMapper.ToDto(category);
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category is null)
            throw new ArgumentException("Categoria não encontrada");

        await _categoryRepository.DeleteAsync(category);
    }

    public async Task<CategoryTotalsResponseDto> GetCategoryTotalsAsync(Guid userId)
    {
        // Get all categories for the user
        var categories = await _categoryRepository.GetByUserIdAsync(userId);

        // Get all expenses for the user
        var expenses = await _expenseRepository.GetByUserIdAsync(userId);

        // Group expenses by category and calculate totals
        var categoryTotals = categories.Select(category =>
        {
            var categoryExpenses = expenses.Where(e => e.CategoryId == category.Id).ToList();

            var totalIncome = categoryExpenses
                .Where(e => e.Type == TransactionType.Income)
                .Sum(e => e.Amount);

            var totalExpenses = categoryExpenses
                .Where(e => e.Type == TransactionType.Expense)
                .Sum(e => e.Amount);

            return new CategoryTotalsDto
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                Balance = totalIncome - totalExpenses
            };
        }).ToList();

        // Calculate grand totals
        var grandTotalIncome = categoryTotals.Sum(c => c.TotalIncome);
        var grandTotalExpenses = categoryTotals.Sum(c => c.TotalExpenses);
        var grandBalance = grandTotalIncome - grandTotalExpenses;

        return new CategoryTotalsResponseDto
        {
            Categories = categoryTotals,
            GrandTotalIncome = grandTotalIncome,
            GrandTotalExpenses = grandTotalExpenses,
            GrandBalance = grandBalance
        };
    }
}
