using CGD.APP.DTOs.Expense;

namespace CGD.APP.Services.Expenses;

public interface IExpenseService
{
    Task<ExpenseDto> CreateAsync(ExpenseCreateDto dto, Guid adminUserId);
    Task<ExpenseDto> GetByIdAsync(Guid id);
    Task<IReadOnlyList<ExpenseDto>> GetByUserIdAsync(Guid userId);
    Task<IReadOnlyList<ExpenseDto>> GetByUserIdAsync(Guid userId, ExpenseFilterDto filter);
    Task<ExpenseDto> UpdateAsync(Guid expenseId, Guid adminId, ExpenseUpdateDto dto);
    Task DeleteAsync(Guid id);
    Task<IReadOnlyList<ExpenseDto>> GetAll(Guid userId);
    Task<ExpenseTotalsResponseDto> GetTotalsByUserIdAsync(Guid userId, ExpenseFilterDto filter);
}
