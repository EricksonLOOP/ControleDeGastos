using CGD.APP.DTOs.Expense;
using CGD.APP.DTOs.User;
using CGD.APP.Mappers;
using CGD.APP.Services.Groups;
using CGD.CrossCutting.Exceptions;
using CGD.CrossCutting.Security;
using CGD.Domain.Entities;
using CGD.Domain.IRepositories;
using Microsoft.CSharp.RuntimeBinder;

namespace CGD.APP.Services.Users;

using CGD.Domain.IRepositories;

public class UserService(
    IUserRepository userRepository, 
    IExpenseRepository expenseRepository, 
    IGroupMemberRepository groupMemberRepository, 
    PasswordHash passwordHash, 
    IGroupService groupService
  
  ) : IUserService


{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IExpenseRepository _expenseRepository = expenseRepository;
    private readonly IGroupMemberRepository _groupMemberRepository = groupMemberRepository;
    private readonly PasswordHash _passwordHash = passwordHash;
    private readonly IGroupService _groupService = groupService;
  
    

    public async Task<UserDto> CreateSimpleAsync(UserSimpleCreateDto dto, Guid groupAdmin)
{
    var user = UserMapper.SimpleCreateToEntity(dto);
    user.Age = CalculateAge(user.BirthDate);
    var userCreated = await _userRepository.AddAsync(user);
    var groupToAdd = await _groupService.GetByIdAsync(dto.GroupId, groupAdmin);
    await _groupService.AddUserToGroupAsync(groupToAdd.Id, groupAdmin,userCreated.Id);
    return UserMapper.ToDto(user);
}
public async Task<IReadOnlyList<UserDto>> GetPagedByCommonGroupsAsync(Guid userId, int page, int pageSize)
{
    if (page <= 0)
        throw new ArgumentException("Page deve ser maior que zero.");
    if (pageSize <= 0 || pageSize > 100)
        throw new ArgumentException("PageSize deve estar entre 1 e 100.");

    // Buscar grupos do usuário logado
    var userGroups = await _groupService.GetAllAsync(userId);
    var groupMembers = userGroups
        .SelectMany(g => g.Members)
        .ToList();
    if (groupMembers.Count == 0)
        return new List<UserDto>();

    // Buscar todos os membros desses grupos
    var allMemberships = groupMembers;
    

    // Pegar usuários únicos
    var userIds = allMemberships.Select(m => m.UserId).Distinct().ToList();
    // Paginar
    var pagedUserIds = userIds.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    var users = new List<UserDto>();
    foreach (var id in pagedUserIds)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user != null)
            users.Add(UserMapper.ToDto(user));
    }
    return users;
}

public async Task<UserDto> GetByIdAsync(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
        throw new UserNotFoundException();

    return UserMapper.ToDto(user);
}

public async Task<IReadOnlyList<UserDto>> GetPagedAsync(int page, int pageSize)
{
    if (page <= 0)
        throw new ArgumentException("Page deve ser maior que zero.");
    if (pageSize <= 0 || pageSize > 100)
        throw new ArgumentException("PageSize deve estar entre 1 e 100.");

    var users = await _userRepository.GetPagedAsync(page, pageSize);
    return users.Select(UserMapper.ToDto).ToList();
}

public async Task<UserDto> CreateAsync(UserCreateDto dto)
{
    var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
    if (existingUser is not null)
        throw new ArgumentException("Email já cadastrado");

    var user = UserMapper.CreateToEntity(dto, _passwordHash.HashPassword(dto.Password));
    user.Age = CalculateAge(user.BirthDate);
    await _userRepository.AddAsync(user);
    return UserMapper.ToDto(user);
}

public async Task<UserDto> UpdateAsync(Guid id, UserUpdateDto dto)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
        throw new UserNotFoundException();

    user.Name = dto.Name;
    // Se atualizar data de nascimento, atualizar idade
    if (dto is { BirthDate: var birthDate })
    {
        user.BirthDate = birthDate;
        user.Age = CalculateAge(birthDate);
    }


await _userRepository.UpdateAsync(user);
    return UserMapper.ToDto(user);
}
private static int CalculateAge(DateTime birthDate)
{
    var today = DateTime.Today;
    var age = today.Year - birthDate.Year;
    if (birthDate.Date > today.AddYears(-age)) age--;
    return age;
}
public async Task DeleteAsync(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
        throw new UserNotFoundException();

    await _userRepository.DeleteAsync(user);
}

public async Task<UserTotalsResponseDto> GetUserTotalsAsync()
{
    // Buscar todos os expenses com os usuários
    var expenses = await _expenseRepository.GetAllWithUsersAsync();

    // Agrupar por usuário
    var userTotalsList = expenses
        .GroupBy(e => new { e.UserId, e.User!.Name })
        .Select(g => new UserTotalsDto
        {
            UserId = g.Key.UserId,
            Name = g.Key.Name,
            TotalIncome = g.Where(e => e.Type == TransactionType.Income).Sum(e => e.Amount),
            TotalExpense = g.Where(e => e.Type == TransactionType.Expense).Sum(e => e.Amount),
            Balance = g.Where(e => e.Type == TransactionType.Income).Sum(e => e.Amount)
                    - g.Where(e => e.Type == TransactionType.Expense).Sum(e => e.Amount)
        })
        .OrderBy(u => u.Name)
        .ToList();

    // Calcular totais gerais
    var overallTotals = new OverallTotalsDto
    {
        TotalIncome = userTotalsList.Sum(u => u.TotalIncome),
        TotalExpense = userTotalsList.Sum(u => u.TotalExpense),
        Balance = userTotalsList.Sum(u => u.Balance)
    };

    return new UserTotalsResponseDto
    {
        UserTotals = userTotalsList,
        OverallTotals = overallTotals
    };
}

public async Task<List<EnrichedUserDto>> GetAllEnrichedUsers(Guid adminGroupId)
{
    var enrichedUsers = await _userRepository.GetEnrichedUsers(adminGroupId);

    var usersId = enrichedUsers.Select(u => u.Id).ToList();

    var usersExpenses = await _expenseRepository.GetByDebtorIdsAsync(usersId);

    var expensesByUser = usersExpenses
        .GroupBy(e => e.DebtorId ?? throw new ArgumentException("O Debtor ID não existe"))
        .ToDictionary(g => g.Key, g => g.Select(ExpenseMapper.ToDto).ToList());

    var result = enrichedUsers.Select(user =>
    {
        var userGroup = user.GroupMembers
            .Select(gm => GroupMapper.ToDto(gm.Group))
            .FirstOrDefault();

        return new EnrichedUserDto
        {
            User = UserMapper.ToDto(user),
            Group = userGroup,
            Expenses = expensesByUser.TryGetValue(user.Id, out var exp)
                ? exp
                : []
        };
    }).ToList();

    return result;
}
}
