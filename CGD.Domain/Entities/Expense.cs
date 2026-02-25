using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CGD.Domain.Entities;

public class Expense
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
 
    [Required]
    [MaxLength(400)]
    public string Description { get; set; } = null!;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; }
    public Guid CategoryId { get; set; }

    [Required]
    public TransactionType Type { get; set; }
    public Guid? DebtorId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(DebtorId))]
    public User? Debtor { get; set; }
    public ExpenseCategory Category { get; set; } = null!;
}
