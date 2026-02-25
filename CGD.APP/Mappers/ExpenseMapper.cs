using CGD.APP.DTOs.Category;
using CGD.APP.DTOs.Expense;
using CGD.Domain.Entities;

namespace CGD.APP.Mappers;

public static class ExpenseMapper
{
    public static Expense ToEntity(ExpenseCreateDto dto)
    {
        return new Expense
        {
            Id = Guid.NewGuid(),
            UserId = dto.UserId,
            Description = dto.Description,
            Amount = dto.Amount,
            Date = DateTime.UtcNow,
            DebtorId = dto.DebtorId,
            CategoryId = dto.CategoryId,
            Type = dto.Type
        };
    }

    public static ExpenseDto ToDto(Expense expense)
    {
        ExpenseDebtorDto? debtorDto = null;
        CategoryDto? categoryDto = null;
        if (expense.Debtor != null)
            debtorDto = UsertToExpenserDebtorDto(expense.Debtor);
        if (expense.Category != null)
            categoryDto = CategoryMapper.ToDto(expense.Category);
        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDebtor = debtorDto,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            Category = categoryDto,
            CategoryId = expense.CategoryId,
            Type = expense.Type
        };
    }

    public static ExpenseDebtorDto UsertToExpenserDebtorDto(User user)
    {
        return new ExpenseDebtorDto
        {
            Id = user.Id,
            Name = user.Name,
        };
    }
    public static ExpenseDto ToDtoWithGroupAndUserData(Expense expense, ExpenseGroup expenseGroup, ExpenseDebtorDto expenseDebtor)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            ExpenseDebtor = expenseDebtor,
            ExpenseGroup = expenseGroup,
            Description = expense.Description,
            Amount = expense.Amount,
            Date = expense.Date,
            CategoryId = expense.CategoryId,
            Type = expense.Type
        };
    }
}

