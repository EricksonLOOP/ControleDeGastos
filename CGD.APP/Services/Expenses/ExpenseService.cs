using System.Xml;
using CGD.APP.DTOs.Expense;
using CGD.APP.Mappers;
using CGD.APP.Services.Groups;
using CGD.CrossCutting.Exceptions;
using CGD.Domain.Entities;
using CGD.Domain.Filters;
using CGD.Domain.IRepositories;

namespace CGD.APP.Services.Expenses;

public class ExpenseService(
    IExpenseRepository expenseRepository,
    IUserRepository userRepository,
    IExpenseCategoryRepository categoryRepository,
    IGroupMemberRepository groupMemberRepository) : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository = expenseRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IExpenseCategoryRepository _categoryRepository = categoryRepository;
    private readonly IGroupMemberRepository _groupMemberRepository = groupMemberRepository;
    public async Task<ExpenseDto> CreateAsync(ExpenseCreateDto dto, Guid adminUserId)
    {
        dto.UserId = adminUserId;
        var debtorUser = await _userRepository.GetByIdAsync(dto.DebtorId) ?? throw new UserNotFoundException("Não encontramos o devedor");
        // valida a idade
        if (debtorUser.Age < 18 && dto.Type == TransactionType.Income)
            throw new InvalidTransactionForMinorException();

        // validação da categoria
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId) ?? throw new CategoryNotFoundException();


        if (category.UserId != dto.UserId)
            throw new CategoryNotFoundException();

        switch (dto.Type)
        {
            // validação da purpose
            case TransactionType.Expense when category.Purpose == CategoryPurpose.Income:
            case TransactionType.Income when category.Purpose == CategoryPurpose.Expense:
                throw new InvalidCategoryPurposeException();
        }

        var expense = ExpenseMapper.ToEntity(dto);
        await _expenseRepository.AddAsync(expense);
        return ExpenseMapper.ToDto(expense);
    }

    public async Task<ExpenseDto> GetByIdAsync(Guid id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense is null)
            throw new ExpenseNotFoundException();

        return ExpenseMapper.ToDto(expense);
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetByUserIdAsync(Guid userId)
    {
        return await GetByUserIdAsync(userId, new ExpenseFilterDto());
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetByUserIdAsync(Guid userId, ExpenseFilterDto filter)
    {
        var expenseFilter = new ExpenseFilter
        {
            CategoryId = filter.CategoryId,
            StartDate = filter.StartDate,
            EndDate = filter.EndDate,
            MinAmount = filter.MinAmount,
            MaxAmount = filter.MaxAmount,
            DescriptionContains = filter.DescriptionContains?.Trim()
        };

        var expenses = await _expenseRepository.GetByUserIdFilteredAsync(userId, expenseFilter);
        return expenses.Select(ExpenseMapper.ToDto).ToList();
    }

    public async Task<ExpenseDto> UpdateAsync(Guid expenseId, Guid adminId, ExpenseUpdateDto dto)
    {
        var expense = await _expenseRepository.GetByIdAsync(expenseId) ??
                      throw new ExpenseNotFoundException();

        // Valida usuário existir
        var user = await _userRepository.GetByIdAsync(expense.DebtorId
                                                      ?? throw new ArgumentException("O Debtor ID não existe"
                                                          ))
            ?? throw new UserNotFoundException("Não encontramos o Debtor");


        // restrição de 18 anos
        if (user.Age < 18 && dto.Type == TransactionType.Income)
            throw new InvalidTransactionForMinorException();

        // Vvalida a categoria
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)
                       ?? throw new CategoryNotFoundException();


        if (category.UserId != adminId)
            throw new CategoryNotFoundException();

        switch (dto.Type)
        {
            // valida purpose
            case TransactionType.Expense when category.Purpose == CategoryPurpose.Income:
            case TransactionType.Income when category.Purpose == CategoryPurpose.Expense:
                throw new InvalidCategoryPurposeException();
        }

        expense.Description = dto.Description;
        expense.Amount = dto.Amount;
        expense.DebtorId = dto.DebtorId;
        expense.CategoryId = dto.CategoryId;
        expense.Type = dto.Type;

        await _expenseRepository.UpdateAsync(expense);
        return ExpenseMapper.ToDto(expense);
    }

    public async Task DeleteAsync(Guid id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense is null)
            throw new ExpenseNotFoundException();

        await _expenseRepository.DeleteAsync(expense);
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetAll(Guid userId)
    {
        var groupsMembers = await _groupMemberRepository.GetAllByGroupAdminId(userId);

        var userIds = groupsMembers
            .Select(gm => gm.User.Id)
            .Distinct()
            .ToList();

        var expenses = await _expenseRepository.GetByUserIdsAsync(userIds);

        return expenses.Select(ExpenseMapper.ToDto).ToList();
    }
}
