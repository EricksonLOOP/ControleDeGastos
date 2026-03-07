using System.ComponentModel.DataAnnotations;
using CGD.Domain.Entities;

namespace CGD.APP.DTOs.Expense;

public class ExpenseCreateDto
{
    [Required]
    // UserId e preenchido no backend pelo usuario autenticado (escopo de ownership).
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(400)]
    public string Description { get; set; } = null!;

    [Required]
    // Valor positivo e obrigatorio para preservar consistencia financeira.
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required]
    public TransactionType Type { get; set; }
    [Required]
    public Guid DebtorId { get; set; }
}
