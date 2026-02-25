using CGD.Domain.Entities;

namespace CGD.Domain.IRepositories;

public interface IExpenseCategoryRepository
{
    Task AddAsync(ExpenseCategory category);
    Task<ExpenseCategory?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ExpenseCategory>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<ExpenseCategory>> GetPagedByUserIdAsync(Guid userId, int page, int pageSize);
    Task UpdateAsync(ExpenseCategory category);
    Task DeleteAsync(ExpenseCategory category);
}
