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
        // Escopo de ownership: o lancamento sempre pertence ao admin autenticado.
        dto.UserId = adminUserId;
        var debtorUser = await _userRepository.GetByIdAsync(dto.DebtorId) ?? throw new UserNotFoundException("Não encontramos o devedor");
        // Regra de negocio: menor de 18 anos nao pode registrar receita.
        if (debtorUser.Age < 18 && dto.Type == TransactionType.Income)
            throw new InvalidTransactionForMinorException();

        // Categoria deve existir antes de validar ownership/finalidade.
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId) ?? throw new CategoryNotFoundException();


        // Categoria precisa pertencer ao mesmo dono da despesa (escopo do admin).
        if (category.UserId != dto.UserId)
            throw new CategoryNotFoundException();

        switch (dto.Type)
        {
            // Regra de compatibilidade: tipo da transacao deve combinar com a finalidade da categoria.
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
        // Mapeia filtro de API para filtro de dominio mantendo validacao centralizada no repositorio.
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

        // Revalida devedor no update para reaplicar regras de menoridade no estado atual.
        var user = await _userRepository.GetByIdAsync(expense.DebtorId
                                                      ?? throw new ArgumentException("O Debtor ID não existe"
                                                          ))
            ?? throw new UserNotFoundException("Não encontramos o Debtor");


        // Regra de negocio igual ao create: menor nao pode registrar receita.
        if (user.Age < 18 && dto.Type == TransactionType.Income)
            throw new InvalidTransactionForMinorException();

        // Vvalida a categoria
        var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)
                       ?? throw new CategoryNotFoundException();


        // Ownership de categoria e validado pelo admin autenticado, nao pelo debtor.
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
        // Mantem semantica de dominio: "nao existe" vira erro explicito, nao delete silencioso.
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense is null)
            throw new ExpenseNotFoundException();

        await _expenseRepository.DeleteAsync(expense);
    }

    public async Task<IReadOnlyList<ExpenseDto>> GetAll(Guid userId)
    {
        // Consolida despesas visiveis a partir dos membros dos grupos administrados pelo usuario.
        var groupsMembers = await _groupMemberRepository.GetAllByGroupAdminId(userId);

        var userIds = groupsMembers
            .Select(gm => gm.User.Id)
            .Distinct()
            .ToList();

        var expenses = await _expenseRepository.GetByUserIdsAsync(userIds);

        return expenses.Select(ExpenseMapper.ToDto).ToList();
    }

    public async Task<ExpenseTotalsResponseDto> GetTotalsByUserIdAsync(Guid userId, ExpenseFilterDto filter)
    {
        // Reuse filtering logic to obtain the subset and aggregate on server side.
        var expenseFilter = new Domain.Filters.ExpenseFilter
        {
            CategoryId = filter.CategoryId,
            StartDate = filter.StartDate,
            EndDate = filter.EndDate,
            MinAmount = filter.MinAmount,
            MaxAmount = filter.MaxAmount,
            DescriptionContains = filter.DescriptionContains?.Trim()
        };

        var expenses = await _expenseRepository.GetByUserIdFilteredAsync(userId, expenseFilter);

        var totalIncome = expenses.Where(e => e.Type == Domain.Entities.TransactionType.Income).Sum(e => e.Amount);
        var totalExpenses = expenses.Where(e => e.Type == Domain.Entities.TransactionType.Expense).Sum(e => e.Amount);

        return new ExpenseTotalsResponseDto
        {
            TotalIncome = totalIncome,
            TotalExpenses = totalExpenses,
            Balance = totalIncome - totalExpenses
        };
    }
}
