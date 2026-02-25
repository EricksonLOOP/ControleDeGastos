using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using CGD.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace CGD.Infra.Repositories;

public class ExpenseCategoryRepository(CGDDbContext context) : IExpenseCategoryRepository
{
    private readonly CGDDbContext _context = context;

    public async Task AddAsync(ExpenseCategory category)
    {
        _context.ExpenseCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task<ExpenseCategory?> GetByIdAsync(Guid id)
    {
        return await _context.ExpenseCategories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetByUserIdAsync(Guid userId)
    {
        return await _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetPagedByUserIdAsync(Guid userId, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        return await _context.ExpenseCategories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.Name)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task UpdateAsync(ExpenseCategory category)
    {
        _context.ExpenseCategories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ExpenseCategory category)
    {
        _context.ExpenseCategories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
