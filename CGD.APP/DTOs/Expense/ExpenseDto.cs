using CGD.APP.DTOs.Category;
using CGD.Domain.Entities;

namespace CGD.APP.DTOs.Expense;

public class ExpenseDto
{
    public Guid Id { get; set; }
 
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }
    public TransactionType Type { get; set; }
    public ExpenseDebtorDto? ExpenseDebtor { get; set; }
    public CategoryDto? Category { get; set; }
    public ExpenseGroup? ExpenseGroup { get; set; }
}
