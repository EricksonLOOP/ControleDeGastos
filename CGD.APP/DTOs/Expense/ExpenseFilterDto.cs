namespace CGD.APP.DTOs.Expense;

public class ExpenseFilterDto
{
    public Guid? CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? DescriptionContains { get; set; }
}
