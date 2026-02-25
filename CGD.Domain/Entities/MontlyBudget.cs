using System.ComponentModel.DataAnnotations;

namespace CGD.Domain.Entities;

public class MonthlyBudget
{
    [Key]
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User? User { get; set; }

    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Limit { get; set; }
}
