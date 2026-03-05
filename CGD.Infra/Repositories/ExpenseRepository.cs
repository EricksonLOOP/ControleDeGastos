using CGD.Domain.Entities;
using CGD.Domain.Filters;
using CGD.Domain.IRepositories;
using CGD.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace CGD.Infra.Repositories;

public class ExpenseRepository(CGDDbContext context) : IExpenseRepository
{
    private readonly CGDDbContext _context = context;

    public async Task AddAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
    }

    public async Task<Expense?> GetByIdAsync(Guid id)
    {
        return await _context.Expenses
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IReadOnlyList<Expense>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Expense>> GetByUserIdFilteredAsync(Guid userId, ExpenseFilter filter)
    {
        var query = _context.Expenses
            .AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.Debtor)
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (filter.CategoryId.HasValue)
            query = query.Where(e => e.CategoryId == filter.CategoryId.Value);

        if (filter.StartDate.HasValue)
            query = query.Where(e => e.Date >= filter.StartDate.Value);


        if (filter.EndDate.HasValue)
            query = query.Where(e => e.Date <= filter.EndDate.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(e => e.Amount >= filter.MinAmount.Value);

        if (filter.MaxAmount.HasValue)
            query = query.Where(e => e.Amount <= filter.MaxAmount.Value);

        if (!string.IsNullOrWhiteSpace(filter.DescriptionContains))
            query = query.Where(e => e.Description.Contains(filter.DescriptionContains));

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<Expense>> GetAllWithUsersAsync()
    {
        return await _context.Expenses
            .AsNoTracking()
            .Include(e => e.User)
            .ToListAsync();
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Expense expense)
    {
        _context.Expenses.Remove(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteByUserOrDebtorIdAsync(Guid userId)
    {
        var expenses = await _context.Expenses
            .Where(e => e.UserId == userId || e.DebtorId == userId)
            .ToListAsync();

        if (expenses.Count == 0)
            return;

        _context.Expenses.RemoveRange(expenses);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Expense>> GetByUserIdsAsync(List<Guid> userIds)
    {
        return await _context.Expenses
            .Where(e => userIds.Contains(e.UserId))
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Expense>> GetByDebtorIdsAsync(List<Guid> debtorsIds)
    {
        var allExpensesToTest = await _context.Expenses.ToListAsync();
        return await _context.Expenses
            .Where(e => e.DebtorId.HasValue && debtorsIds.Contains(e.DebtorId.Value))
            .Include(e => e.Category)
            .ToListAsync();
    }
}
