using CGD.APP.DTOs.Expense;
using CGD.APP.DTOs.Group;

namespace CGD.APP.DTOs.User;

public class EnrichedUserDto
{
    public UserDto? User { get; set; }
    public GroupDto? Group { get; set; }
    public List<ExpenseDto>? Expenses { get; set; }
}