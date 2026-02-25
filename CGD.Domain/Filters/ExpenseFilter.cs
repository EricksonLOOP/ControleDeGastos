namespace CGD.Domain.Filters;

public class ExpenseFilter
{
    public Guid? CategoryId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? DescriptionContains { get; set; }
}
