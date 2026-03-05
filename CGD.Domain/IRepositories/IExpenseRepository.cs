using CGD.Domain.Entities;
using CGD.Domain.Filters;

namespace CGD.Domain.IRepositories;

public interface IExpenseRepository
{
    Task AddAsync(Expense expense);
    Task<Expense?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<Expense>> GetByUserIdFilteredAsync(Guid userId, ExpenseFilter filter);
    Task<IReadOnlyList<Expense>> GetAllWithUsersAsync();
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(Expense expense);
    Task DeleteByUserOrDebtorIdAsync(Guid userId);
    Task<IReadOnlyList<Expense>> GetByUserIdsAsync(List<Guid> userIds);
    Task<IReadOnlyList<Expense>> GetByDebtorIdsAsync(List<Guid> userIds);

}
