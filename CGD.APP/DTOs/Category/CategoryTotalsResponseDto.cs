namespace CGD.APP.DTOs.Category;

public class CategoryTotalsResponseDto
{
    public IReadOnlyList<CategoryTotalsDto> Categories { get; set; } = new List<CategoryTotalsDto>();
    public decimal GrandTotalIncome { get; set; }
    public decimal GrandTotalExpenses { get; set; }
    public decimal GrandBalance { get; set; }
}
